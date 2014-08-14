using System;
using System.Threading;
using System.Collections.Generic;

namespace Movetron
{
	//Анимированный объект
	class TAnimated
	{	
		public string Type;
		public int x, y;		//0..77, 0..31
		public int dx, dy;		//-1, 0 ,1
		public char RepreChar;	//Символ, которым отображается	
		public bool Visible;	//Видимость игроком
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
			if (Geo.Map[x+dx,y+dy] == Geo.C_WALL ||
			   	Geo.Map[x+dx,y+dy] == Geo.C_METAL)
				return false;
			this.x = this.x + this.dx;
			this.y = this.y + this.dy;
			
			return true;
		}
		//попал ли под огонь?
		public bool FireDeath()
		{
			if (Geo.Map[x,y] == Geo.C_FIRE)
				return true;
			else 
				return false;				
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
			this.Type = "THero";
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
			//Стартовая позиция уже в памяти
			Geo.Map[x,y] = Geo.C_EMPTY;
		}
	}
		
	//Враг
	class TEnemy : TAnimated
	{
		public enum TMode 			//Режим поведения
		{
			Stand, Patrol, Pursue, Hunt, Dead
		};
		public TMode Mode;					
		public bool SeeHero;		//Видит ли героя
		//Конструктор
		public TEnemy(int x, int y)
		{
			this.Type = "THero";
			this.SetXY(x,y);
			this.SetDXDY(0,0);
			this.SetLook((char)001,11);
			this.Visible = true;
			this.SeeHero = false;
			this.Mode = TMode.Stand;		//Стоим, потом решаем дальше
			//стартовая позиция уже в памяти
			Geo.Map[x,y] = Geo.C_EMPTY;
		}
	}
	//Бомба
	class TBomb : TAnimated
	{
		public uint TimeStarted;
		public bool Active;
		
		//Зажигает огонь
		private void SetFire(int x, int y)
		{
			//Если огонь за пределами карты
			if (x > Geo.MAXX || x < 0 || y > Geo.MAXY || y < 0)
				return;
			
			if (Geo.Map[x,y] == Geo.C_EMPTY || Geo.Map[x,y] == Geo.C_WALL)
				Geo.Map[x,y] = Geo.C_FIRE;
			CRT.TextColor(Geo.Code2Col[Geo.Map[x,y]]);
			CRT.PutChar(x+1,y+1,Geo.Code2Look[Geo.Map[x,y]]);						
		}
		//Гасит огонь
		private void SetCool(int x, int y)
		{
			//Если огонь за пределами карты или попался не тайл огня
			if (x > Geo.MAXX || x < 0 || y > Geo.MAXY || y < 0 || Geo.Map[x,y] != Geo.C_FIRE)
				return;
			
			Geo.Map[x,y] = Geo.C_EMPTY;
			CRT.TextColor(Geo.Code2Col[Geo.Map[x,y]]);
			CRT.PutChar(x+1,y+1,Geo.Code2Look[Geo.Map[x,y]]);		
		}
		//Если d true, создаем, если false затираем
		private void Explosion(int x,int y, bool d)
		{
			if (d == true)
			{
				SetFire(x,y);
				SetFire(x,y-2);SetFire(x,y+2);
				SetFire(x,y-1);SetFire(x,y+1);
				SetFire(x-2,y);SetFire(x+2,y);
				SetFire(x-1,y);SetFire(x+1,y);
				SetFire(x+1,y+1);SetFire(x-1,y-1);
				SetFire(x-1,y+1);SetFire(x+1,y-1);
			} else
			{
				SetCool(x,y);
				SetCool(x,y-2);SetCool(x,y+2);
				SetCool(x,y-1);SetCool(x,y+1);
				SetCool(x-2,y);SetCool(x+2,y);
				SetCool(x-1,y);SetCool(x+1,y);
				SetCool(x+1,y+1);SetCool(x-1,y-1);
				SetCool(x-1,y+1);SetCool(x+1,y-1);
			}
		}		
		//Отсчет бомбы и эффекты взрыва
		public void Tick(uint time)
		{
			if (Active == false)
				return;
			CRT.TextColor(11);
			CRT.Print(x+1, y+1, (3-(Math.Abs(time - TimeStarted))/10).ToString());
			
			if (Math.Abs(time - TimeStarted) > 30)
			{
				Geo.Map[x,y] = Geo.C_EMPTY;
				Explosion(x,y,true);
			}
			if (Math.Abs(time - TimeStarted) > 32)
			{
				Explosion(x,y,false);
				Active = false;
			}
		}
		public TBomb(int x, int y, uint TimeStarted)
		{
			this.Type = "TBomb";
			this.SetXY(x,y);
			this.SetDXDY(0,0);
			this.SetLook(Geo.Code2Look(Geo.C_BOMB),Geo.Code2Col(Geo.C_BOMB));
			this.Visible = true;
			this.Active = true;
			this.TimeStarted = TimeStarted;
			//Сделать элементом карты, чтобы не затиралось
			//движением героя и врагов.
			Geo.Map[x,y] = Geo.C_BOMB; 
		}		
	}
	//Основной класс
	class MovetronGame
	{			
				
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
		static public bool ExitLevel;				//Завершить уровень
		
		/*сделай функцию статическую в классе MovetronGame
		public static bool CheckStep(TMap map, КлассПерсонажа hero, int x, int y)
		внутри неё ты уже можешь делать проверку типа который имеет hero*/
		
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
			
			InitBox.InitScreen();	//Инициализируем экран.
			InitBox.LegalStuff()	;//Юридическая фигня.    				 		    		
			InitBox.LangSelect()	;//Выбор языка.
			
			GameMenu.Run();
			///Здесь будет начинаться цикл уровней
			///В начале цикла загрузка карты, инициализация экземпляров и т.д
			///В конце - очистка листов бомб, врагов...
			do
			{
				ExitLevel = false;
				Console.Clear();
        	
				//Загрузка карты
				Geo.LoadMap(Geo.Map,LevelNumber);
				MapParsedData = ParseMap(Geo.Map);
        	
				//Сколько камушков на карте
				GemsLeft = MapParsedData.GemsLeft;
				//Создание героя
				THero Hero = new THero(MapParsedData.HeroStartX,MapParsedData.HeroStartY);
				//Бомбы
			
				List <TBomb> BombList = new List <TBomb> {};
				Hero.Draw(true);
				GamePanel.Draw();
				GamePanel.ShowMazeNumber(LevelNumber);
				GamePanel.ShowGems(GemsLeft);
				GamePanel.ShowBombs(Hero.Bombs);
				GamePanel.ShowAmmo(Hero.Ammo);
				GamePanel.ShowLives(Hero.Lives,false);
				Geo.ShowMap(Geo.Map,true);
				GamePanel.SendMsg(GamePanel.MSG_WELCOME);
        	
        		//ЦИКЛ УРОВНЯ
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
							case ConsoleKey.B:
							{
								if (Hero.Bombs > 0)
								{
									BombList.Add(new TBomb(Hero.x,Hero.y,time));
									BombList[BombList.Count-1].Draw(true);
									Hero.Bombs--;
									GamePanel.ShowBombs(Hero.Bombs);
									GamePanel.SendMsg(GamePanel.MSG_BOMB_PLANTED);
								}
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
					
					//ВСЯЧИНА НА КАРТЕ=========================================
					//Герой попал в огонь?
					if (Hero.FireDeath() == true)
					{					
						Hero.x = MapParsedData.HeroStartX;
						Hero.y = MapParsedData.HeroStartY;
						Hero.Lives--;
						GamePanel.SendMsg(GamePanel.MSG_DEAD);
						GamePanel.ShowLives(Hero.Lives,false);						
					}			
					//Нашел красный ключ
					if (Geo.Map[Hero.x,Hero.y] == Geo.C_REDKEY)
					{
						Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
						Hero.RedKey = true;
						GamePanel.SendMsg(GamePanel.MSG_RED_KEY);
						GamePanel.ShowKeys(Hero.RedKey,Hero.GreenKey,Hero.BlueKey);						
					}
					//Нашел зеленый ключ
					if (Geo.Map[Hero.x,Hero.y] == Geo.C_GREENKEY)
					{
						Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
						Hero.GreenKey = true;
						GamePanel.SendMsg(GamePanel.MSG_GREEN_KEY);
						GamePanel.ShowKeys(Hero.RedKey,Hero.GreenKey,Hero.BlueKey);						
					}
					//Нашел синий ключ
					if (Geo.Map[Hero.x,Hero.y] == Geo.C_BLUEKEY)
					{
						Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
						Hero.BlueKey = true;
						GamePanel.SendMsg(GamePanel.MSG_BLUE_KEY);
						GamePanel.ShowKeys(Hero.RedKey,Hero.GreenKey,Hero.BlueKey);						
					}
					//Дополнительная жизнь
					if (Geo.Map[Hero.x,Hero.y] == Geo.C_LIFE)
					{
						Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
						Hero.Lives++;
						GamePanel.SendMsg(GamePanel.MSG_FOUND_LIFE);
						GamePanel.ShowLives(Hero.Lives,false);
					}
										
					//Герой подобрал камень?
					if (Geo.Map[Hero.x,Hero.y] == Geo.C_GEM)
					{
						Geo.Map[Hero.x,Hero.y] = Geo.C_EMPTY;
						GemsLeft--;
						GamePanel.SendMsg(GamePanel.MSG_GEMFOUND);
						GamePanel.ShowGems(GemsLeft);
					}
    				//Если собраны все камни
    				if (GemsLeft == 0)
    					{
    						GamePanel.SendMsg(GamePanel.MSG_COMPLETE);
    						//Можно на следующий уровень
    						if (Geo.Map[Hero.x,Hero.y] == Geo.C_EXIT)
    						{
    							LevelNumber++;
    							ExitLevel = true;
    						}
    					}
    							
					//Обход бомб
					foreach(TBomb i in BombList)
						if (i.Active == true) 
							i.Tick(time);
									
					//Простой таймер
					if (time < 64000) time++;
					else time = 0;
    			
					Thread.Sleep(100);		//одинаковая скорость на всех компах
    		  
				} while(ExitLevel == false); //цикла уровня
			} while (true); //главного цикла
		} // of Main()
	} //of MovetronGame
} //of namespace
