using System;
using System.IO;
using System.Threading;

class MoveEdit
{	
	static int MazeNumber = 1;
	//Установить тайл кода Piece на x,y 
	static void SetPiece(int x, int y, byte Piece)
	{
		Geo.Map[x,y] = Piece;
		CRT.TextColor(Geo.Code2EdCol[Geo.Map[x,y]]);
		CRT.PutChar(x+1,y+1,Geo.Code2Look[Piece]);		
	}
	//Показать номер уровня
	static void ShowMazeNumber()
	{
		CRT.TextColor(15);
		CRT.Print(38,0,CRT.AddZeroes(MazeNumber,2));
	}
	//Рисует оформление
	static void DrawBordersAndInfo()
	{
		int i;
		CRT.TextColor(11);
		CRT.Print(0,0,"╔════════════════════════════════════╬  ╬══════════════════════════════════════╗");
		//Рамки
		for (i=1;i<=32;i++)
		{
			CRT.Print(0,i,"║");
			CRT.Print(79,i,"║");
		}
		for (i=1;i<=78;i++)
				CRT.Print(i,33,"═");
		//Уголки
		CRT.Print(0,33,"╚");
		CRT.Print(79,33,"╝");
		Console.BackgroundColor = ConsoleColor.Blue;
		CRT.TextColor(11);
		CRT.Print(0,38,"  F1-SAVE MAP    F2-LOAD MAP    PgUp-NEXT MAP    PgDn-PREVIOUS MAP    Esc-EXIT  ");
		Console.MoveBufferArea(0,38,80,1,0,39);
		Console.BackgroundColor = ConsoleColor.Black;
		CRT.Print(0,38,"                                                                                ");
		CRT.Print(0,36," W-Wall   S-Gem   Space-Clear  E-Metal wall   X-Exit position  Z-Start position ");
		CRT.Print(0,37,"  Q-Enemy   1-Red key   2-Green key   3-Blue key   A-Ammo  B-Bomb  4-Red door   ");
		CRT.Print(0,38,"                       5-Green door  6-Blue door  L-Life                        ");
	}
	//#################################################################################################
	static void Main()
	{
		int x = 5, y = 5;
		int dx = 0, dy = 0;
		int StartX = 0, StartY = 0;
		int ExitX = 0, ExitY = 0;
		//Есть ли вход и выход
		bool HasStart = false, HasExit = false;
		//Сохранен ли новый уровень, при его наличии?
		bool NewMazeSaved = true; 
    	
		//Init Screen
		Console.SetWindowSize(80,40);
		Console.SetBufferSize(80,40);
		Console.CursorVisible = false;
		Console.Clear();
		DrawBordersAndInfo();
		ShowMazeNumber();
		Geo.EmptyMap(Geo.Map);
		//editor loop
		do
		{
			//Если нажата клавиша
			if (Console.KeyAvailable == true)
			{
				//Смотрим, что за клавиша
				CRT.KeyPressed = Console.ReadKey(true);   								
				switch (CRT.KeyPressed.Key)
				{
					case ConsoleKey.LeftArrow:
					{	dx = -1; dy = 0; break; }
					case ConsoleKey.RightArrow:
					{	dx = 1; dy = 0; break; }
					case ConsoleKey.UpArrow:
					{	dx = 0; dy = -1; break; }
					case ConsoleKey.DownArrow:
					{	dx = 0; dy = 1; break; }
					case ConsoleKey.W:		//wall
					{	SetPiece(x,y,Geo.C_WALL);break;}
					case ConsoleKey.S:		//gem
					{	SetPiece(x,y,Geo.C_GEM);break;}
					case ConsoleKey.E:		//concrete
					{	SetPiece(x,y,Geo.C_METAL);break;}					
					case ConsoleKey.Q:			//enemy_spawn
					{	SetPiece(x,y,Geo.C_SPAWN);break;}
					case ConsoleKey.D1:	//red key
					{	SetPiece(x,y,Geo.C_REDKEY);break;}
					case ConsoleKey.D2:	//green key
					{	SetPiece(x,y,Geo.C_GREENKEY);break;}
					case ConsoleKey.D3:	//blue key
					{	SetPiece(x,y,Geo.C_BLUEKEY);break;}
					case ConsoleKey.D4:	//red door
					{	SetPiece(x,y,Geo.C_REDDOOR);break;}
					case ConsoleKey.D5:	//green key
					{	SetPiece(x,y,Geo.C_GREENDOOR);break;}
					case ConsoleKey.D6:	//blue door
					{	SetPiece(x,y,Geo.C_BLUEDOOR);break;}
					case ConsoleKey.A:	//spare ammo
					{	SetPiece(x,y,Geo.C_SPARE_AMMO);break;}
					case ConsoleKey.B:	//spare bomb
					{	SetPiece(x,y,Geo.C_SPARE_BOMB);break;}
					case ConsoleKey.L:	//spare life
					{	SetPiece(x,y,Geo.C_LIFE);break;}
					case ConsoleKey.Spacebar:	//empty
					{	
						if (Geo.Map[x,y] == Geo.C_START)
							HasStart = false;
						if (Geo.Map[x,y] == Geo.C_EXIT)
							HasExit = false;
						SetPiece(x,y,Geo.C_EMPTY);
						break;						
					}
					case ConsoleKey.Z: //start position
					{
						if (Geo.Map[StartX,StartY] == Geo.C_START)
							SetPiece(StartX,StartY,Geo.C_EMPTY);
						StartX = x;
						StartY = y;
						SetPiece(StartX,StartY,Geo.C_START);
						HasStart = true;						
						break;
					}
					case ConsoleKey.X: //exit position
					{
						if (Geo.Map[ExitX,ExitY] == Geo.C_EXIT)
							SetPiece(ExitX,ExitY,Geo.C_EMPTY);
						ExitX = x;
						ExitY = y;
						SetPiece(ExitX,ExitY,Geo.C_EXIT);
						HasExit = true;
						break;
					}
					case ConsoleKey.F1:  	//save map
					{
						if (HasStart == false)
							{
								CRT.Print(23,34,"LEVEL MAZE HAS NO START FOR PLAYER");
								Thread.Sleep(500);
								CRT.Print(23,34,"                                  ");								
							}
						else if (HasExit == false)
							{
								CRT.Print(24,34,"LEVEL MAZE HAS NO EXIT FOR PLAYER");
								Thread.Sleep(500);
								CRT.Print(24,34,"                                 ");
							}
						else 
							{
								Geo.SaveMap(Geo.Map,MazeNumber);
								NewMazeSaved = true;
							}
						
						break;
					}
					case ConsoleKey.F2:  	//load & show map
					{
						Geo.LoadMap(Geo.Map,MazeNumber);
						Geo.ShowMap(Geo.Map,true);
						break;
					}
					case ConsoleKey.PageUp:
					{
						if (MazeNumber <  Geo.LastMazeNumber())
						{
							MazeNumber++;
							ShowMazeNumber();
						} else if (NewMazeSaved == true)
						{
							//Новый пустой уровень
							Geo.EmptyMap(Geo.Map);
							Geo.ShowMap(Geo.Map,true);
							MazeNumber++;
							ShowMazeNumber();
							Geo.SaveMap(Geo.Map,MazeNumber);
							//Сохранили, сброс флага
							NewMazeSaved = false;
						}
						break;    					
					}
					case ConsoleKey.PageDown:
					{
						if (MazeNumber > 1) 
						{
							MazeNumber--;    					
							ShowMazeNumber();
						}
						break;
					}
					case ConsoleKey.Escape
					{
						//exit is here
						break;
					}
				}

				CRT.FlushKeyboardBuffer();		//чтобы не забивать нажатиями
			} //Закончили опрос клавиатуры. 
    		
			//Рисуем курсор
			if (x + dx >= 0 && x + dx <= Geo.MAXX && 
			y + dy >= 0 && y + dy <= Geo.MAXY)
			{
				CRT.TextColor(Geo.Code2EdCol[Geo.Map[x,y]]);
				CRT.PutChar(x+1,y+1,Geo.Code2Look[Geo.Map[x,y]]);
				x = x + dx;
				y = y + dy;    			
				dx = 0;
				dy = 0;
				CRT.TextColor(15);
				CRT.PutChar(x+1,y+1,'█');    			    		
			}
			Thread.Sleep(25);
	
		} while(true); //main loop
	}
}
