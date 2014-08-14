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
	public const byte C_METAL = 5;
	public const byte C_BOMB = 6;
	public const byte C_FIRE = 7;
	public const byte C_EXIT = 8;
	public const byte C_REDKEY = 9;
	public const byte C_GREENKEY = 10;
	public const byte C_BLUEKEY = 11;
	public const byte C_REDDOOR = 12;
	public const byte C_GREENDOOR = 13;
	public const byte C_BLUEDOOR = 14;
	public const byte C_SPARE_AMMO = 15;
	public const byte C_SPARE_BOMB = 16;
	public const byte C_LIFE = 17;
	
	//Символ по коду ячейки
	static public char [] Code2Look = new char [] {' ','▓','',
	'','','┘','¤','▒','▓','','','','#','#','#','','¤',''};
	//Цвет по коду ячейки (игра)
	static public byte [] Code2Col = new byte[] {0,12,9,0,0,10,15,14,11,
	12,10,9,12,10,9,15,15,12};
	//Цвет по коду ячейки (редактор)
	static public byte [] Code2EdCol = new byte[] {0,12,9,14,11,10,15,14,11,
	12,10,9,12,10,9,15,15,12};
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
				if (Editor == true)
				{
					//в редакторе видно всё
					CRT.TextColor(Code2EdCol[Map[i,j]]);
					CRT.PutChar(i+1,j+1,Code2Look[Map[i,j]]);
				}
				else 
				{	
					//изначально видны в игре только стены и камушки
					if (Map[i,j] == C_WALL || Map[i,j] == C_METAL 
					||Map[i,j] == C_GEM)
						CRT.PutChar(i+1,j+1,Code2Look[Map[i,j]]);				
				}
			
	}
	//Возвращает номер последнего уровня карты
	static public int LastMazeNumber()
	{
		int FileSize;
		FileStream MapFile = new FileStream(MAP_NAME, FileMode.Open);
		FileSize = (int)MapFile.Length;
		MapFile.Close();
		return FileSize / MAZE_SIZE;  
	}
}
