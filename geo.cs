using System.IO;

//Общие для игры и редактора данные и методы карты 
static public class Geo
{
	//Внутреннее представление ячеек карты
	public const byte C_EMPTY = 0;
	public const byte C_WALL = 1;
	public const byte C_GEM = 2;
	public const byte C_START = 3;
	public const byte C_SPAWN = 4;
	//Внешний вид ячеек карты
	public const char L_EMPTY = ' ';
	public const char L_WALL = '▓';
	public const char L_GEM = '';
	public const char L_START = '';
	public const char L_SPAWN = '';		
	//Символ по коду ячейки
	static public char [] Code2Look = new char [] {' ','▓','','',''};
	//Цвет по коду ячейки
	static public byte [] Code2Col = new byte[] {0,12,9,0,0};  
			
	//Игровая карта
	public const string MAP_NAME = "movetron.dat";
	public const int MAXX = 77;	//0..77  = 78
	public const int MAXY = 31; //0..31 = 32
	public const int MAZE_SIZE = (MAXX+1)*(MAXY+1); //2496
	public const int MAX_ENEMIES = 25;
	static public byte [,] Map = new byte [Geo.MAXX+1,Geo.MAXY+1];		
	
	//Создает пустую карту в массиве Map
	static public void EmptyMap(byte [,] Map)
	{
    	int i, j;
		for(i=0;i <= MAXX;i++)
    		for(j=0;j <= MAXY;j++)
    			Map[i,j] = C_EMPTY;
	}
    	
	//Сохраняет карту в файл из массива Map
	static public void SaveMap(byte [,] Map, int Maze)
	{
		int i, j;
		FileStream MapFile = new FileStream(MAP_NAME, FileMode.OpenOrCreate);
		MapFile.Seek((Maze-1)*MAZE_SIZE,SeekOrigin.Begin);
		for (i=0;i <= MAXX;i++)
			for (j=0;j <= MAXY;j++)
				MapFile.WriteByte(Map[i,j]);
							
		MapFile.Close();
	}
		
	//Загрузка карты из файла в массив Map
	static public void LoadMap(byte [,] Map, int Maze)
	{
		int i, j;
		byte Buf;
		FileStream MapFile = new FileStream(MAP_NAME, FileMode.Open);
		MapFile.Seek((Maze-1)*MAZE_SIZE,SeekOrigin.Begin);
		for (i=0;i <= MAXX;i++)
			for (j=0;j <= MAXY;j++)
				{
					Buf = (byte)MapFile.ReadByte();
					Map[i,j] = Buf;
				}
		MapFile.Close();
	}	
	
	//Показывает карту
	//Если Editor true, то выводит все ячейки, если false то только стены.
	static public void ShowMap(byte [,] Map, bool Editor)
	{
		int i, j;
		for (i=0;i <= MAXX;i++)
			for (j=0;j <= MAXY;j++)
			{
				CRT.TextColor(Code2Col[Map[i,j]]);
				if (Editor == true)
					CRT.PutChar(i+1,j+1,Code2Look[Map[i,j]]);
				else if (Map[i,j] == C_WALL)
					CRT.PutChar(i+1,j+1,Code2Look[Map[i,j]]);				
			}
				
	}
	//Возвращает номер последнего уровня карты
	static public int LastMazeNumber()
	{
		int LMN;
		FileStream MapFile = new FileStream(MAP_NAME, FileMode.Open);
		LMN = (int)MapFile.Length;
		MapFile.Close();
		return LMN / MAZE_SIZE;  
	}
}
		
