using System.Resources;
public class MazeCreation
{
    static ResourceManager resourceManager4 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    
    private Cell[,] maze;
    private int size;
    private Random rand = new Random();
    private List<Trap> traps = new List<Trap>(); 
    public (int x, int y) exit;
    public (int x, int y)? cooldownReductionTile = null;
    public (int x, int y)? speedIncreaseTile = null; 
    private (int x, int y) player1Pos;
    private (int x, int y) player2Pos;
    public (int x, int y)? randomTeleportPortal = null; 
    public int Size => size;

    public MazeCreation(int size)
    { 
        this.size = size;
        maze = new Cell[size,size];
        
        do
        {
            for(int i=0; i< size;i++)
            {
                for(int j=0; j<size;j++)
                {
                    maze[i,j]=new Cell(false); // Todo paredes
                }
            }
        
            GenerateMaze(0, 0);
        }while(!IsMazeReachable());

        SetExit();
        GeneratesTraps();
        BeneficialTilesGeneration();
    }
    private void GenerateMaze(int x, int y) // recursive backtracking 
    {
        var directions = new (int dx, int dy)[]{(1, 0), (-1, 0), (0, 1), (0, -1)};

        Shuffle(directions); //asegura aleatoriedad
        
        maze[x, y].isOpen = true; // Marcar la celda como pasillo

        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx * 2; // Mover 2 celdas en esa direccion
            int ny = y + dy * 2; // saltar la pared

            if (nx >= 0 && nx < size && ny >= 0 && ny < size && !maze[nx, ny].isOpen)
            {
                maze[x + dx, y + dy].isOpen = true;  //Quitar la pared entre las celdas 
                GenerateMaze(nx, ny); 
            }
        }

        if (!HasOpenCell(size - 1))  //Asegurar por lo menos una celda abierta en la ultima fila para la salida
        {
            int col = rand.Next(size);
            maze[size - 1, col].isOpen = true;
        }
    }
    private bool HasOpenCell(int row)
    {
        for (int col = 0; col < size; col++)
        {
            if (maze[row, col].isOpen)
                return true;
        }
        return false;
    }
    private void Shuffle((int dx, int dy)[] directions) //Fisher-Yates shuffle,In-Place Shuffling, cada elemento es cambiado una vez
    {
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1); //cada elemento desde el inicio del array hasta i tiene la misma probabilidad de ser escogida
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }

    }
    public bool IsMazeReachable() //Asegurar que cada casilla sea alcanzable
    {
        int[,] distances = new int[size, size];
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                distances[x, y] = maze[x, y].isOpen ? -1 : -10;
            }
        }

        if (maze[0, 0].isOpen)
        {
            distances[0, 0] = 0;
        }

        bool modified;
        int currentDistance = 0;

        do
        {
            modified = false;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (distances[x, y] == currentDistance)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            int nx = x + dx[d];
                            int ny = y + dy[d];

                            if (nx >= 0 && nx < size && ny >= 0 && ny < size && distances[nx, ny] == -1)
                            {
                                distances[nx, ny] = currentDistance + 1;
                                modified = true;
                            }
                        }
                    }
                }
            }

            currentDistance++;
        } while (modified);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (distances[x, y] == -1)
                {
                    return false; // Encontró una celda inalcanzable
                }
            }
        }

        return true; // Todas las casillas son alcanzables
    }

    public void SetPlayer1Position(int x, int y) 
    {
        player1Pos = (x, y);
    }
    public void SetPlayer2Position(int x, int y)
    {
        player2Pos = (x, y);
    }
    public (int x, int y) GetPlayer1Position()
    {
        return player1Pos;
    }
    public (int x, int y) GetPlayer2Position()
    {
        return player2Pos;
    }    
    public class Cell
    {
        public bool isOpen;
        public Cell(bool shouldBeOpen)
        {
            isOpen=shouldBeOpen;
        }
    }
    private void GeneratesTraps()
    {
        int trapCount = 0; 
        int totalTraps = 4; 

        string[] trapEffects = {
            "Pierdes 1 turno",          // Efecto de trampa 1
            "Vas a 0,0",    
            "Reduce tu velocidad" ,       
            "Aumentatu tiempo de enfriamiento"
        };

        string[] trapEmojis = {
            "🔥", 
            "🕳️", 
            "🐝", 
            "🦇"   
        };

        List<(int x, int y)> validPositions = new List<(int x, int y)>(); //Todas las posiciones validas para trampas
        List<Trap> trapsList = new List<Trap>();

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (maze[i, j].isOpen && !(i == 0 && j == 0)&& !(i == exit.x && j == exit.y))//Una posicion valida es un pasillo que no sea la salida
                {
                    validPositions.Add((i, j)); //(i,j) IGUAL exit HERE
                }
            }
        }
        var validPositionsArr = validPositions.ToArray(); // Convertir a un array para el shuffling

        Shuffle(validPositionsArr);

        foreach (var (x, y) in validPositionsArr) // Poner trampa en la primera posicion valida en totalTraps
        {
            if (trapCount < totalTraps)
            {
                string trapName = "T" + (trapCount + 1); //Nombre "T1", "T2", "T3"
                string effect = trapEffects[trapCount]; // Tomar su efecto
                string emoji = trapEmojis[trapCount];

                trapsList.Add(new Trap(x, y, trapName, effect,emoji));
                trapCount++;
            }

            if (trapCount == totalTraps)
                break;
        }
        traps = trapsList;  // Asignar la lista de trampas al objeto MazeGeneration 
    }
    public Trap? IsTrapAtPosition(int i, int j)
    {
        foreach (var trap in traps)
        {
            if (trap.X == i && trap.Y == j) // HERE 
            {
                
                return trap; 
            }
        }
        return null; 
    }

public bool IsExitReachable(int exitRow, int exitCol) //Algortimo de Lee
{
        bool[,] visited=new bool[size,size];

        visited[0,0]=true; //marcar la celda inicial como abierta
        int[] dr={-1,1,0,0,};
        int[] dc={0,0,1,-1};

        bool change;
        do
        {
            change=false;
            for(int r=0;r<size;r++)
            {
                for(int c=0;c<size;c++)
                {
                    if(!visited[r,c]) continue; //saltar las celdas sin marcar
                    for(int d=0;d<dr.Length;d++)//chequear celdas vecinas a [r,c]
                    {
                        int vr=r+dr[d];
                        int vc=c+dc[d];

                        if(ValidPosition(size,vr,vc) && !visited[vr,vc] && maze[vr,vc].isOpen)//determinar si el vecino es valido y no ha sido chequeado
                        {
                            visited[vr,vc]=true;
                            change=true;
                            if(vr==exitRow&&vc==exitCol)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        while(change);
        return false; //salida no es alcanzable
    }
    private void SetExit()
    {
        int exitRow = size - 1;

        for (int col = 0; col < size; col++)// Ver si hay una casilla abierta alcanzable en la ultima fila
        {
            if (maze[exitRow, col].isOpen && IsExitReachable(exitRow, col))
            {
                exit = (exitRow, col);
                return;
            }
        }

        for (int col = 0; col < size; col++) //abrir una casilla aleatoria en la ultima fila y asegurar que sea alcanzable
        {
            if (maze[exitRow, col].isOpen || !maze[exitRow, col].isOpen)
            {
                maze[exitRow, col].isOpen = true; 
                if (IsExitReachable(exitRow, col))
                {
                    exit = (exitRow, col);
                    return;
                }
                maze[exitRow, col].isOpen = false; //si no es alcanzable, cerrarla 
            }
        }
        int randomCol = rand.Next(size);  //como ultima instancia abrir una celda aleatoria como salida
        maze[exitRow, randomCol].isOpen = true;
        exit = (exitRow, randomCol);
    }
    public static bool ValidPosition(int size,int row,int col)
    {
        return row >= 0 && row < size && col >= 0 && col < size;
    }    
    public bool IsWall(int row, int col)
    {
        if (row < 0 || row >= maze.GetLength(0) || col < 0 || col >= maze.GetLength(1))// Chequear si esta fuera del laberinto
        {
            string? outOfBoundsMessage = resourceManager4.GetString("OutOfBoundsMessage");
            if (!string.IsNullOrEmpty(outOfBoundsMessage))
            {
                Console.WriteLine(string.Format(outOfBoundsMessage, row, col));
            }
            return true; 
        }

        bool isWall = !maze[row, col].isOpen; 
        return isWall;
    }
    private void BeneficialTilesGeneration()
    {
        List<(int x, int y)> validPositions = new List<(int x, int y)>();
            
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (maze[i, j].isOpen && !(i == exit.x && j == exit.y) && IsTrapAtPosition(i, j) == null) // HERE
                {
                    validPositions.Add((i, j));
                }
            }
        }

        if (validPositions.Count >= 2)
        {
            var validPositionsArr = validPositions.ToArray();
            Shuffle(validPositionsArr);

            Random random=new Random();
            
            cooldownReductionTile = validPositions[random.Next(size-1)]; // Assignar las casillas beneficiosas
            speedIncreaseTile = validPositions[random.Next(size-1)];
        }
    }
    public bool IsBeneficialTile(int x, int y, out string tileType)
    {
        if (cooldownReductionTile.HasValue && cooldownReductionTile.Value == (x, y))
        {
            tileType = "Cooldown Reduction";
            return true;
        }
        else if (speedIncreaseTile.HasValue && speedIncreaseTile.Value == (x, y))
        {
            tileType = "Speed Increase";
            return true;
        }

        tileType = string.Empty;
        return false;
    }
    public void GenerateTeleportationPortal()
    {
        List<(int x, int y)> validPositions = new List<(int x, int y)>();

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (IsValidPositionForPortal(i, j))
                {
                    validPositions.Add((i, j));  
                }
            }
        }

        if (validPositions.Count > 0)
        {
            var validPositionsArr = validPositions.ToArray();
            Shuffle(validPositionsArr); 
            randomTeleportPortal = validPositionsArr[0];  
        }
    }
    private bool IsValidPositionForPortal(int x, int y) //chequea si una posicion es valida para colocar el portal de teleportation
    {
        if (maze[x, y].isOpen && !(x == exit.x && y == exit.y)  && IsTrapAtPosition(x, y) == null && !IsBeneficialTile(x, y, out _)) // Chequea si una posicion no es una pared, la salida, no es una trampa o casilla beneficiosa 
        {
            return true;  // HERE
        }
        return false;  
    }
    public void CheckTeleportation(Player player)
    {
        if (player.Position == randomTeleportPortal)
        {
            string? teleportMessage = resourceManager4.GetString("TeleportRandom");
            if (!string.IsNullOrEmpty(teleportMessage))
            {
                Console.WriteLine(string.Format(teleportMessage, player.Name));
            }
            player.Position = GetRandomValidPosition();  
        }
    }
    public (int x, int y) GetRandomValidPosition()
    {
        List<(int x, int y)> validPositions = new List<(int x, int y)>();

        for (int i = 0; i < size; i++) //Encontrar todas las posiciones validas
        {
            for (int j = 0; j < size; j++)
            {
                if (maze[i, j].isOpen && !(i == exit.x && j == exit.y))
                {
                    validPositions.Add((i, j));
                }
            }
        }

        if (validPositions.Count > 0)// Seleccionar una posicion aleatoria de la lista
        {
            Random rand = new Random();
            int index = rand.Next(validPositions.Count);
            return validPositions[index];
        }
        return (0, 0);  
    }
}