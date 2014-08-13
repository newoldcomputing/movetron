using System;
using System.IO;

class MoveEdit
{				
        static int MazeNumber = 1;
	
        static void SetPiece(int x, int y, byte Piece)
	{
		Geo.Map[x,y] = Piece;
		CRT.TextColor(Geo.Code2EdCol[Geo.Map[x,y]]);
		CRT.PutChar(x+1,y+1,Geo.Code2Look[Piece]);		
	}
	
	static void ShowMazeNumber()
	{
		CRT.TextColor(15);
		CRT.Print(38,0,CRT.AddZeroes(MazeNumber,2));
	}
	
	//#################################################################################################
	static void Main()
	{   	
                int x = 5, y = 5;
    	        int dx = 0, dy = 0;
    	        int HeroX = 0, HeroY = 0;
    	        //Сохранен ли новый уровень, при его наличии?
    	        bool NewMazeSaved = true; 
    	
                //Init Screen
                Console.SetWindowSize(80,40);
                Console.SetBufferSize(80,40);
                Console.CursorVisible = false;
                Console.Clear();
                CRT.TextColor(11);
                CRT.Print(0,0,"╔════════════════════════════════════╬  ╬══════════════════════════════════════╗");
                
                ShowMazeNumber();
                Geo.EmptyMap(Geo.Map);
                //editor loop
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
                                                dx = -1;
                                                dy = 0;
                                                break;
                                        }
                                        case ConsoleKey.RightArrow:
                                        {
                                                dx = 1;
                                                dy = 0;
                                                break;
                                        }
                                        case ConsoleKey.UpArrow:
                                        {
                                                dx = 0;
                                                dy = -1;
                                                break;
                                        }
                                        case ConsoleKey.DownArrow:
                                        {
                                                dx = 0;
                                                dy = 1;
                                                break;
                                        } 
                                        case ConsoleKey.W:		//wall
                                        {
                                                SetPiece(x,y,Geo.C_WALL);    					
                                                break;
                                        }
                                        case ConsoleKey.S:		//gem
                                        {
                                                SetPiece(x,y,Geo.C_GEM);    					
                                                break;    					
                                        }
                                        case ConsoleKey.E:		//concrete
                                        {
                                                SetPiece(x,y,Geo.C_CONCRETE);    					
                                                break;    					
                                        }	
                                        case ConsoleKey.Spacebar:	//empty
                                        {
                                                SetPiece(x,y,Geo.C_EMPTY);    					    					
                                                break;
                                        }
                                        case ConsoleKey.Q:	//enemy_spawn
                                        {
                                                SetPiece(x,y,Geo.C_SPAWN);    					    					
                                                break;
                                        }
                                        case ConsoleKey.Z: //start position
                                        {
                                                if (Geo.Map[HeroX,HeroY] == Geo.C_START) 
                                                        SetPiece(HeroX,HeroY,Geo.C_EMPTY);
                                                HeroX = x;
                                                HeroY = y;
                                                SetPiece(HeroX,HeroY,Geo.C_START);    					
                                                break;
                                        }
                                        case ConsoleKey.F1:  	//save map
                                        {
                                                Geo.SaveMap(Geo.Map,MazeNumber);
                                                NewMazeSaved = true;
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
                                }

                        CRT.FlushKeyboardBuffer();		//чтобы не забивать нажатиями
                } //Закончили опрос клавиатуры. Через порты было круче.
    	
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
                        System.Threading.Thread.Sleep(25);
	
                } while(true); //main loop
        }
}
