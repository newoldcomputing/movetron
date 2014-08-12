using System;
using System.Threading;
using System.Collections.Generic;

namespace Movetron
{
	//Анимированный объект
	class TAnimated
	{	
		public int x, y;			   //0..77, 0..31
		public int dx, dy;	   		   //-1, 0 ,1
		public char RepreChar;		   //Символ, которым отображается	
		public bool Visible;           //Видимость игроком
		public byte Color;
	
		//Задает координаты
		public void SetXY(int x, int y)
		{
			this.x = x;
			this.y = y;
		}		
			
		//Задает скорость
		public void SetDXDY(int dx, int dy)
		{
			this.dx = dx;
			this.dy = dy;
		}
			
		//Обновляет координаты
		//true, если это возможно			
		public bool UpdateXY()
		{
			//Не вышел ли за пределы поля?
			if (x + dx < 0 || x + dx > Geo.MAXX || y + dy < 0 || y + dy > Geo.MAXY)
				return false;
			
			//Препятствия
			if (Geo.Map[x+dx,y+dy] == Geo.C_WALL)
				return false;
			
			this.x = this.x + this.dx;
			this.y = this.y + this.dy;
			return true;
		}
			
		//Задать представление на экране и цвет
		public void SetLook(char RepreChar, byte Color)
		{
			this.RepreChar = RepreChar;
			this.Color = Color;					
		}		
			
		//true - отрисовка на экране.
		//false - затирание.			
		public void Draw(bool d)
		{
			if (Visible == false)			//если невидим 
				d = false; 
			
			if (d == true) 
			{ 
				CRT.TextColor(Color);
				CRT.PutChar(x+1,y+1,RepreChar);
			}
			else 
			{
				CRT.TextColor(Geo.Code2Col[Geo.Map[x,y]]);
				CRT.PutChar(x+1,y+1,Geo.Code2Look[Geo.Map[x,y]]);
			}
		}	
	}
	//Протагонист
	class THero : TAnimated
	{
		public int Lives;
		public int Bombs, Ammo;
		public bool RedKey, BlueKey, GreenKey;	
		//Конструктор
		public THero(int x, int y)
		{
			this.SetXY(x,y);
			this.SetDXDY(0,0);
			this.SetLook((char)001,14);
			this.Visible = true; //Игрок видит себя всегда
			this.Lives = 3;
			this.Bombs = 2;
			this.Ammo = 3;
			this.RedKey = false;
			this.BlueKey = false;
			this.GreenKey = false;			
		}
	}
		
	//Враг
	class TEnemy : TAnimated
	{
		public enum TMode 			//Режим поведения
		{
			Stand, Patrol, Pursue, Hunt
		};
		public TMode Mode;					
		public bool SeeHero;		//Видит ли героя
		//Конструктор
		public TEnemy(int x, int y)
		{
			this.SetXY(x,y);
			this.SetDXDY(0,0);
			this.SetLook((char)001,11);
			this.Visible = true;
			this.SeeHero = false;
			this.Mode = TMode.Stand;		//Стоим, потом решаем дальше
		}
	}

	//Основной класс
	class MovetronGame
	{			
		//Сообщения
	 	public const byte MSG_DEAD = 0;
	 	public const byte MSG_WELCOME = 1;
		public const byte MSG_GEMFOUND = 2;
		public const byte MSG_COMPLETE = 3;
		public const byte MSG_PAUSE = 17;
				
		//Прочитано из массива карты
		public struct TMapParsedData
		{
			public int HeroStartX; 
			public int HeroStartY;
			public int [] EnemyX; 
			public int [] EnemyY;
			public int GemsLeft;						
		}
		
		static uint time = 0;
		static public int LevelNumber = 1; 
		
		static public TMapParsedData MapParsedData; //Прочитано из массива
		static public int GemsLeft;					//Осталось камней
		
		//Парсит массив карты
		static TMapParsedData ParseMap(byte [,] Map)
		{
			int i,j;
			int EnemyCounter = 0;
			TMapParsedData Buffer = new TMapParsedData();
			
			Buffer.EnemyX = new int[Geo.MAX_ENEMIES];
			Buffer.EnemyY = new int[Geo.MAX_ENEMIES];
			Buffer.GemsLeft = 0;
			
			for (i=0;i<=Geo.MAXX;i++)
				for (j=0;j<Geo.MAXY;j++)
				{
					if (Geo.Map[i,j] == Geo.C_START)
					{
						Buffer.HeroStartX = i;
						Buffer.HeroStartY = j;
					}
					if (Geo.Map[i,j] == Geo.C_SPAWN)
					{
						Buffer.EnemyX[EnemyCounter] = i;
						Buffer.EnemyY[EnemyCounter] = j;
						EnemyCounter++;
					}
					if (Geo.Map[i,j] == Geo.C_GEM)
					    Buffer.GemsLeft++;
					    
					    
				}
			
			return Buffer;
		}
		//#################################################################################################
		static void Main()
    	{   	
    		InitBox.InitScreen();  	//Инициализируем экран.
    		InitBox.LegalStuff(); 	//Юридическая фигня.    				 		    		
    		InitBox.LangSelect(); 	//Выбор языка.
			
    		GameMenu.Run();
        	Console.Clear();
        	
        	//Загрузка карты
        	Geo.LoadMap(Geo.Map,LevelNumber);
        	MapParsedData = ParseMap(Geo.Map);
        	
        	//Сколько камушков на карте
        	GemsLeft = MapParsedData.GemsLeft;
        	
        	//Создание героя
        	THero Hero = new THero(MapParsedData.HeroStartX,MapParsedData.HeroStartY);
        	Hero.Draw(true);
        	
        	GamePanel.Draw();
        	GamePanel.ShowMazeNumber(LevelNumber);
        	GamePanel.ShowGems(GemsLeft);
        	GamePanel.ShowBombs(Hero.Bombs);
        	GamePanel.ShowAmmo(Hero.Ammo);
        	GamePanel.ShowLives(Hero.Lives,false);
        	
        	Geo.ShowMap(Geo.Map,true);
        	
        	
    		//ГЛАВНЫЙ ЦИКЛ
    		do
    		{	
    			//Если нажата клавиша
    			if (Console.KeyAvailable == true)
    			{
    				//Смотрим, что за символ
    				CRT.KeyPressed = Console.ReadKey(true);   				
    				switch (CRT.KeyPressed.Key)
    				{
    					case ConsoleKey.LeftArrow:
    					{
    						Hero.SetDXDY(-1,0);
    						break;
    					}
    					case ConsoleKey.RightArrow:
    					{
    						Hero.SetDXDY(1,0);
    						break;
    					}
    					case ConsoleKey.UpArrow:
    					{
    						Hero.SetDXDY(0,-1);
    						break;
    					}
    					case ConsoleKey.DownArrow:
    					{
    						Hero.SetDXDY(0,1);
    						break;
    					} 
    					case ConsoleKey.Escape:
    					{
    						if (GamePanel.Quit() == true)
    						{
    							Console.Clear();
    							System.Environment.Exit(0);
							}							
							break;							
    				 	}
    					case ConsoleKey.F1:
    					{
    						GamePanel.Pause();
    						break;
    					}
    			
    				}
    				CRT.FlushKeyboardBuffer();		//чтобы не забивать нажатиями
    			} //Закончили опрос клавиатуры. Через порты было круче.
    					    					   			
    			    			   			
    			//Смещение игрока
    			Hero.Draw(false);
    			Hero.UpdateXY();
    			Hero.SetDXDY(0,0);
    			Hero.Draw(true);
    			
    			//Нашли камушек?
    			if (Geo.Map[Hero.x,Hero.y] == Geo.C_GEM)
    			{
    				Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
    				GemsLeft--;
    				GamePanel.SendMsg(MSG_GEMFOUND);
    				GamePanel.ShowGems(GemsLeft);
    			}
    			
    			//Простой таймер
    			if (time < 64000) time++;
    			else time = 0;
    			
    			Thread.Sleep(200); 		//одинаковая скорость на всех компах
    		  
    		} while(true); //главного цикла  		
		}
	}
}
