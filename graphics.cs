using System;
using System.Threading;
using System.Collections.Generic;

namespace Movetron
{
	//Инициализация игры до построения игрового поля.
	static public class InitBox
	{
		public enum TLanguage {ENG, RUS}
		public static TLanguage Language;
		
		//Инициализация экрана.
		static public void InitScreen()
		{
			Console.Title = "Movetron v2.0";
    		Console.SetWindowSize(80,40);  //[0..79], [0..39]
    		Console.SetBufferSize(80,40);  //[0..79], [0..39]
    		Console.CursorVisible = false;	
		}
		
		//Показ юридической фигни
		static public void LegalStuff()
		{
			CRT.TextColor(8);
			CRT.Print(29,2,"THIS GAME IS FREEWARE");
			CRT.Print(3,35,"The name has nothing to do with Finnish eurodance band (www.movetron.fi)");
			CRT.Print(3,36,"I just picked a random \"robotic\" name back in 2000, without being able  ");
			CRT.Print(3,37,"to check it via Internet, for I didn't have access to it. Pure coincidence.");		
		}
		
		//Выбор языка.
		static public void LangSelect()
		{
			CRT.TextColor(7);
			CRT.Print(32,15,"SELECT LANGUAGE :");
			CRT.Print(34,18,"F1 - ENGLISH");
			CRT.Print(34,21,"F2 - RUSSIAN");
			while (true)
			{
				CRT.KeyPressed = Console.ReadKey(true);
				switch (CRT.KeyPressed.Key)
				{
					case ConsoleKey.F1:
					{
						Language = TLanguage.ENG;
						Console.Clear();
						return;
					}
					case ConsoleKey.F2:
					{
						Language = TLanguage.RUS;
						Console.Clear();
						return;
					}
				}
			}
		}	//of LangSelect()		
	}	//of class InitBox
	
	//Панель игровой инфы
	static public class GamePanel
	{
		static string [] Panel = new string[7]
		{
			"╠═╦═╦══╤═══╤══════╤════╤══════╤═══╤═══╤══╤═══════╤════╦═╦════════════════════╦═╣",
			"║╬╟ ╫┬─┴───┴──┐ ┌─┼─┬──┴──┬──┐└┐  │ ┌─┴──┴─┬──┐  │  ┌─╢╬║                    ║╬║",
			"╟─╫ ╢│        ├─┘ │┌┤     │  │ └┐ └┐│      │  ├──┼──┘ ╟─╢                    ╟─╢",
			"║╬╟ ╢├───┬──┬─┼┐██┼┘├──┬──┴┬─┘  │  │└────┬─┴─┬┘ ┌┘ ┌──╢╬║                    ║╬║",
			"╟─╟ ╫┴┬─┐└┐ │ │├──┼─┤ ╔╧═══╧══╦═╧══╧╗  ╔═╧═══╧╦═╧══╧╗ ╠═╬═                  ═╬═╣",
			"║╬╟ ╫─┼┐├─┼─┼─┼┴──┘┌┼─╢       ║     ╟┐┌╢      ║     ╟─╢╬║                    ║╬║",
			"╚═╩═╩═╧╧╧═╧═╧═╧════╧╧═╩═══════╩═════╩╧╧╩══════╩═════╩═╩═╩═                  ═╩═╝"
		};
		static public string[,] FeedMsgs = new string[,]
		{
			{"NO COMMUNICATION..", "НЕТ СВЯЗИ"},
			{"MOVETRON ONLINE...", "МОДУЛЬ НА СВЯЗИ"},
			{"GEM TELEPORTED", "КАМЕНЬ ОТПРАВЛЕН"},
			{"ALL GEMS COLLECTED","ВСЕ КАМНИ СОБРАНЫ"},
			{"ENEMY DOWN", "ЦЕЛЬ ПОРАЖЕНА"},
			{"BOMB PLANTED","ЗАЛОЖЕНА БОМБА"},
			{"BOMB DETONATED","БОМБА ВЗОРВАЛАСЬ"},
			{"BLASTER CHIP","МОДУЛЬ БЛАСТЕРА"},
			{"BLASTER AMMO","ЗАРЯД БЛАСТЕРА"},
			{"RED KEYCARD","КРАСНЫЙ КЛЮЧ"},
			{"GREEN KEYCARD","ЗЕЛЕНЫЙ КЛЮЧ"},
			{"BLUE KEYCARD","СИНИЙ КЛЮЧ"},
			{"GAME LOADED","ИГРА ЗАГРУЖЕНА"},
			{"GAME SAVED","ИГРА СОХРАНЕНА"},
			{"HARDCORE MODE","РЕЖИМ ХАРДКОРА"},
			{"ENEMY SPOTTED","ВИЖУ ПРОТИВНИКА"},
			{"YOU'RE SPOTTED","ТЕБЯ ЗАСЕКЛИ"}
		};
		static int CurPos = 34;				 //Позиция курсора в экранчике.		
					
		//Рисует прогресс-бар
		static private void DrawProgressBar(int x,int y, int p)
		{
			string Fullbar = "░▒▓▓█";
			int i;
			CRT.TextColor(11);
			//Очистка
			for (i=0;i<5;i++)
				CRT.Print(x+i,y," ");
			//Защита от переполнения
			if (p > 5) p = 5;
			//Вывод
			for (i=0;i<p;i++)
				CRT.Print(x+i,y,Fullbar[i].ToString());
			
		}
		
		//Показать, сколько осталось бомб.
		static public void ShowBombs(int Bombs)
		{
			DrawProgressBar(31,38,Bombs);
        }
		
		//Показать, сколько осталось патронов.
		static public void ShowAmmo(int Ammo)
		{
			DrawProgressBar(47,38,Ammo);
        }
		
		//Показать, сколько собрано камней.
		static public void ShowGems(int Gems)
		{
			CRT.Print(27,35,"  ");
			CRT.TextColor(11);
			CRT.Print(27,35,CRT.AddZeroes(Gems,2));
		}
		
		//Показать номер лабиринта.
		static public void ShowMazeNumber(int MazeNumber)
		{
			CRT.Print(44,35,"  ");
			CRT.TextColor(11);
			CRT.Print(44,35,CRT.AddZeroes(MazeNumber,2));
		}
		
		//Показать, сколько осталось жизней.
		static public void ShowLives(int r, bool Delay)
		{
			int i;
			//Защита от переполнения
			if (r > 5) r = 5; 
				
			//Негорящие лампочки
			CRT.TextColor(2);
			for (i=0;i<5;i++)
				CRT.Print(3,38-i,"▄");
			//Горящие
			CRT.TextColor(10);
			for (i=0;i<r;i++)
			{
				CRT.Print(3,38-i,"▄");
				if (Delay == true) Thread.Sleep(100);
			}
		}
		
		//Показать ключи в наличии
		static public void ShowKeys(bool RedKey, bool GreenKey, bool BlueKey)
		{
			string KeyImg = "▄▀▄▀";
			//Clear
			CRT.Print(58,38,"				              ");
			if (RedKey == true)
			{
				CRT.TextColor(7);
				CRT.Print(58,38,"[");
				CRT.Print(63,38,"]");
				CRT.TextColor(12);
				CRT.Print(59,38,KeyImg);
				
			}
			if (GreenKey == true)
			{
				CRT.TextColor(7);
				CRT.Print(64,38,"[");
				CRT.Print(69,38,"]");
				CRT.TextColor(10);
				CRT.Print(65,38,KeyImg);
			}
			if (BlueKey == true)
			{
				CRT.TextColor(7);
				CRT.Print(70,38,"[");
				CRT.Print(75,38,"]");
				CRT.TextColor(9);
				CRT.Print(71,38,KeyImg);
			}		
		}
		
		//Сообщение на экранчик (text feed)
		//Если Reset true - сброс в исходное состояние, игнор MsgNum
		static public void SendMsg(int MsgNum)
		{
			//Прокрутка
			if (CurPos == 37)
			{				 
				Console.MoveBufferArea(58,35,18,2,58,34);
				CurPos = 36;
			}
			CRT.TextColor(11);
			CRT.Print(58,CurPos,FeedMsgs[MsgNum,(byte)InitBox.Language]);
			if (CurPos < 37) CurPos++;
			
		}
		
		//Очистка/сброс экранчика
		static private void ClearMiniScreen()
		{
			int i;
			for (i=0;i<=2;i++)
				CRT.Print(58,34+i,"                  ");
			CurPos = 34;
		}
		
		//Пауза
		static public void Pause()
		{
			ClearMiniScreen();
			CRT.TextColor(11);
			if (InitBox.Language == InitBox.TLanguage.ENG)
				CRT.Print(61,35,"*P*A*U*S*E*");
			else 
				CRT.Print(61,35,"=П=А=У=З=А=");
			
			do 
			{
			} while (Console.ReadKey(true).Key != ConsoleKey.F1);
			
			ClearMiniScreen();
		}
		
		//Выход?
		static public bool Quit()
		{
			ClearMiniScreen();
			CRT.TextColor(11);
			if (InitBox.Language == InitBox.TLanguage.ENG)
				CRT.Print(59,35,"QUIT GAME? (Y/N)");
			else 
				CRT.Print(61,35,"ВЫЙТИ ? (Y/N)");
			do
			{
				CRT.KeyPressed = Console.ReadKey(true);
				if (CRT.KeyPressed.Key == ConsoleKey.Y)
					return true;
				if (CRT.KeyPressed.Key == ConsoleKey.N)
					{
						ClearMiniScreen();
						return false;
					}
			} while (true);
		}
		
		//Рисует статичную часть панели
		static public void Draw()
		{			
			int i;
			//Выводит рисунок панели
			CRT.TextColor(8);
			for (i=0;i<=6;i++)
				CRT.Print(0,32+i,Panel[i]);
			//Обходим фичу с нежелательной прокруткой экрана при установке курсора
			//правый нижний угол
			Console.MoveBufferArea(0,32,80,7,0,33);
			//ISA-подобный разъем для ключей
			CRT.TextColor(14);
			CRT.Print(58,37,"╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦");
			CRT.Print(58,39,"╩╩╩╩╩╩╩╩╩╩╩╩╩╩╩╩╩╩");
			//Надписи
			CRT.TextColor(12);
			if (InitBox.Language == InitBox.TLanguage.ENG)
			{
				CRT.Print(7,35,"ROBOTS^^^^^^^^^GEMS^^^^^^^^^^^^MAZE");
				CRT.TextColor(12);
				CRT.Print(24,38,"BOMBS^^^^^^^^^^^^AMMO");
				CRT.TextColor(8);
			} else
			{
				CRT.Print(7,35,"РОБОТЫ^^^^^^^^КАМНИ^^^^^^^^^^^^БАЗА");
				CRT.TextColor(12);
				CRT.Print(24,38,"БОМБЫ^^^^^^^^^^^ЗАРЯДЫ");
				CRT.TextColor(8);
			}
			//Рамки
			for (i=1;i<=32;i++)
			{
				CRT.Print(0,i,"║");
				CRT.Print(79,i,"║");
			}
			for (i=1;i<=78;i++)
				CRT.Print(i,0,"═");
			//Уголки
			CRT.Print(0,0,"╔");
			CRT.Print(79,0,"╗");
		}
	}
	//Меню игры
	static public class GameMenu
	{
		const int x = 10;
		const int y = 3;
		//Маска для вывода лого
		static string [] Logo = new string[]
		{
		 	"1111 1111	                                                 ",
		 	"1111 1111                         111                       ",
		 	"1111 1111                         111                       ",
		 	"1111 1111  11111  111 111  11111  1111 111 1  11111  111 11 ",
		 	"11111 111 1111111 111 111 1111111 1111 11111 1111111 1111111",
		 	"11111 111 111 111 111 111 111 111 111  11111 111 111 111 111",
		 	"111 1 111 111 111  11 11  1111111 111  111   111 111 111 111",
		 	"111 1 111 111 111  11 11  1111111 111  111   111 111 111 111",
		 	"111 1 111 111 111  11 11  111     111  111   111 111 111 111",
		 	"111 1 111 111 111  11 11  111 111 111  111   111 111 111 111",
		 	"111 1 111 111 111  11 11  111 111 111  111   111 111 111 111",
		 	"111 1 111 111 111   111   1111111 1111 111   1111111 111 111",
		 	"111 1 111  11111    111    11111  1111 111    11111  111 111"
		};
		
		//Сообщения в меню
		static string[,] MenuTxt = new string[,]
		{
			{"by", "от"},
			{"Press [ENTER] to start", "Нажмите [ENTER] для начала"},
			{"[ESC] to exit and [F1] for help", "[ESC] - покинуть игру, [F1] - помощь"},
			{"[F2] to enter a level code", "[F2] чтобы ввести пароль уровня"},
			{"ENTER YOUR CODE : ", "ВВЕДИТЕ ПАРОЛЬ: "},
			{"WRONG PASSCODE", "НЕВЕРНЫЙ ПАРОЛЬ"},
			{"WELCOME TO MAZE ", "БАЗА НОМЕР "},
			{"DUMMY", "DUMMY"}
		};
		//Сдвиг сообщений в меню
		static int[,] MenuShift = new int[,]
		{
			{0,0}, 
			{0,-2},
			{0,-2}, 
			{0,-2},
			{0,1}, 
			{0,0},
			{0,3},
			{16,14}
			
		};
		//Индексы сообщений  меню и их сдвигов по X
		const byte MENU_BY = 0;
		const byte MENU_ENTER = 1;
		const byte MENU_ESC_OR_F1 = 2;
		const byte MENU_F2 = 3;
		const byte MENU_ENTERCODE = 4;
		const byte MENU_WRONGCODE = 5;
		const byte MENU_WELCOME_LEVEL = 6;
		const byte MENU_MAZE_NUM_SHIFT = 7;
			
		//Для анимации логотипа
		//Заменяет все маркированные mark символы в строке s на символ c
		static private string ChCharStr(string s, char c, char mark)
		{
			int i;
			string Buf = "";	
			for (i=0;i < s.Length;i++)
				if (s[i] == mark) Buf = Buf + c;
				else Buf = Buf + s[i];
			return Buf;
		}
		//Сообщение в меню
		static private void MenuText(int x, int y, int MsgIndex)
		{
			CRT.Print(x+MenuShift[MsgIndex,(byte)InitBox.Language],y,
			          MenuTxt[MsgIndex,(byte)InitBox.Language]);
		}
		//Рисует заставку и текст под ней
		static private void DrawLogoAndMenu(bool FirstTime)
		{
			int i, j;
			char [] LogoChars = new char [] {'░','▒','▓','█','▓'};
			for (j=0;j < 5;j++)
			{
				CRT.TextColor(11);
				for (i=0;i < 8;i++)
					if (FirstTime == true)
						CRT.Print(x,y + i,ChCharStr(Logo[i],LogoChars[j],'1'));
					else 				
        				CRT.Print(x,y + i,ChCharStr(Logo[i],'▓','1'));
				CRT.TextColor(3);
        		for (i=8;i < 13;i++)
        			if (FirstTime == true)
						CRT.Print(x,y + i,ChCharStr(Logo[i],LogoChars[j],'1'));
					else 				
        				CRT.Print(x,y + i,ChCharStr(Logo[i],'▓','1'));
        		Thread.Sleep(200*Convert.ToInt32(FirstTime));
			}
			Thread.Sleep(300*Convert.ToInt32(FirstTime));
			CRT.TextColor(12);
        	CRT.Print(x,y+00,"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄  ");
			CRT.Print(x,y+14,"▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄ ▌▄^^^^^^^^^^^^^^^^^^^^^^^^^  ");
			Thread.Sleep(500*Convert.ToInt32(FirstTime));		
			
			CRT.TextColor(8);
			MenuText(x+20,y+18,MENU_BY);
			CRT.TextColor(4);	 
			CRT.Print(x+23,y+18,"NEW OLD COMPUTING");
			CRT.TextColor(8);
			CRT.Print(x+22,y+20," (C) 2000, 2014");
				
			CRT.TextColor(7);
			MenuText(x+19,y+30,MENU_ENTER);
			MenuText(x+14,y+32,MENU_ESC_OR_F1);
			MenuText(x+17,y+34,MENU_F2);			
		}
		
		//Предлагает ввод пароля на уровень
		static private void AskPassword()
		{
			string PassCode;
			int TempLevelNumber;
			List <Char> LegalInput = new List <Char> {'0','1','2','3','4','5','6','7',
				'8','9','A','B','C','D','E','F'};
			List <string> PassCodes = new List<string> 
			{
				"FC3F","EE4F","38AA","4960","6FE6","5D63","B7AB","47F7","25B5","1765",
				"3244","1A53","1CF1","4C77","2577","C912","D927","6C85","8CBF","5F53",
				"6412","CC04","F71E","262D","A9DE","5AEE","FBC9","6FD6","28BD","DB00",
				"AD73","02C4","5BF1","8D2D","4564","7FF8","0DF3","D39A","B835","79AA",
				"584E","72BB","BF98","D2AF","C931","B162","25E3","1466","F95E","2F08",
				"48EF","5118","1E58","0D98","B2ED","8E11","64EF","C282","3863","4C9A",
				"D88C","254C","BE5B"
			};
			
			CRT.TextColor(15);
			MenuText(29,28,MENU_ENTERCODE);
			CRT.TextColor(12);
			PassCode = CRT.AdvRead(47,28,4,'.',LegalInput);
			CRT.Print(29,28,"                      ");
			if (PassCode == "") return;
			TempLevelNumber = PassCodes.IndexOf(PassCode)+2;
			CRT.Print(1,1,TempLevelNumber.ToString());
			//Проверка легальности пароля
			if (TempLevelNumber > Geo.LastMazeNumber() || TempLevelNumber == -1)
			{
				CRT.TextColor(12);
				MenuText(33,28,MENU_WRONGCODE);
				Thread.Sleep(5000);
				CRT.Print(29,28,"                      ");
			}
			else
			{	
				MovetronGame.LevelNumber = TempLevelNumber;
				CRT.TextColor(15);
				MenuText(31,28,MENU_WELCOME_LEVEL);
				CRT.TextColor(11);
				CRT.Print(31+MenuShift[MENU_MAZE_NUM_SHIFT,(byte)InitBox.Language],28,
				          CRT.AddZeroes(MovetronGame.LevelNumber,2).ToString());
			}				         				
		}
		
		//Выводит меню игры
		static public void Run()
		{						
			int i;
			bool MenuRountine = true;
			
			DrawLogoAndMenu(true);
			//Ожидаем клавиши
			do
			{
				CRT.KeyPressed = Console.ReadKey(true);	
				
				switch (CRT.KeyPressed.Key)
				{
					case ConsoleKey.Escape:
					{
						System.Environment.Exit(0);
						break;
					}
					case ConsoleKey.Enter:
					{
						//Помигать в стиле NES
						for (i=0;i < 10;i++)
						{
							CRT.TextColor(15);
							MenuText(x+19,y+30,MENU_ENTER);
							Thread.Sleep(50);
							CRT.TextColor(7);
							MenuText(x+19,y+30,MENU_ENTER);
							Thread.Sleep(50);
						}
						MenuRountine = false;						
						break;
					}
					case ConsoleKey.F2:
					{
						AskPassword();
						break;
					}
					case ConsoleKey.F1:
					{
						Console.Clear();
						CRT.Print(2,2,"Let there be help");
						Console.ReadKey();
						Console.Clear();
						DrawLogoAndMenu(false);
						break;
					}
				}	//of switch								
			} while (MenuRountine == true);
								
		}	//of Run()
	}	// of class GameMenu
			
} // of namespace Movetron
