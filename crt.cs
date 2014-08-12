//Для работы с дисплеем и клавиатурой.
using System;
using System.Collections.Generic;
	
static public class CRT
{
	static public ConsoleKeyInfo KeyPressed;
		
	//Задает цвет символов (кодировка цветов максимально приближена к MS-DOS)
	static public void TextColor(byte Col)
        {
		ConsoleColor [] OldToNewCol = new ConsoleColor [] 
		{
			ConsoleColor.Black,ConsoleColor.DarkBlue,
			ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, 
			ConsoleColor.DarkRed, ConsoleColor.DarkMagenta,
			ConsoleColor.DarkYellow, ConsoleColor.Gray,
			ConsoleColor.DarkGray,ConsoleColor.Blue,
			ConsoleColor.Green, ConsoleColor.Cyan, 
			ConsoleColor.Red, ConsoleColor.Magenta,
			ConsoleColor.Yellow, ConsoleColor.White
				
		};
		Console.ForegroundColor = OldToNewCol[Col];
	}
		
	//Сброс буфера клавиатуры 
	static public void FlushKeyboardBuffer()
	{
  		while (Console.KeyAvailable) 
  			KeyPressed = Console.ReadKey(true);
  	}
		
	//Улучшенный Read
	static public string AdvRead(int x, int y, int Lim, char Blank, List<Char> Legal)
	{
		string Buf = String.Empty;
		int CarPos = 0;
		int i;
			
		for (i=0;i<Lim;i++)
			PutChar(x+i,y,Blank);
			
		Console.SetCursorPosition(x,y);
		do
		{
			CRT.KeyPressed = Console.ReadKey(true);
			if (CRT.KeyPressed.Key == ConsoleKey.Backspace)
			{
				if (CarPos > 0)
				{
					CarPos--;
					PutChar(x+CarPos,y,Blank);
					Console.SetCursorPosition(x+CarPos,y);
					Buf = Buf.Remove(Buf.Length-1,1);
				}
			}
			else if (CRT.KeyPressed.Key == ConsoleKey.Enter)
			{
				return Buf;	
			}
			else if (CarPos < Lim && Legal.IndexOf((char)CRT.KeyPressed.Key) != -1)
			{
				PutChar(x+CarPos,y,(char)CRT.KeyPressed.Key);
				CarPos++;
				Buf = Buf + (char)CRT.KeyPressed.Key;					
			}
			else if (CRT.KeyPressed.Key == ConsoleKey.Escape)
			         return "";
		} while (true);
				
	}
	//Для простоты
	//Символы '^' игнорируются при выводе, только сдвиг курсора 
	static public void Print(int x, int y, string s)
	{
		int i;
		for(i=0;i < s.Length;i++)
		{
			Console.SetCursorPosition(x+i,y);
			if (s.Substring(i,1) != "^") Console.Write(s.Substring(i,1));
		}
	}
		
	//Вывод одного символа
	static public void PutChar(int x, int y, char c)
	{
		Console.SetCursorPosition(x,y);
		Console.Write(c);
	}
		
	//Добавляет нули. Holders - общее кол-во знакомест.
	static public string AddZeroes(int r, int Holders)
	{
		int i;
		string s;
		string bs = "";
		s = r.ToString();
		for (i=0;i < (Holders-s.Length);i++)	
			bs = bs+"0";
		return bs+s;
	}
}
