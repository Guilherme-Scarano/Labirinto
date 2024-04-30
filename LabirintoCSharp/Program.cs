using System;
using System.Collections.Generic;

namespace LabirintoCSharp
{
    internal class Program
    {
        private const int N = 15; // Tamanho da matriz do labirinto (N x N)
        private const double ProbabilidadeParede = 0.3; // Probabilidade de uma célula ser uma parede

        private static readonly Random rand = new Random();

        private static char[,] labirinto = new char[N, N];
        private static Ponto rato = new Ponto(0, 0);
        private static Ponto queijo = new Ponto(0, 0);
        private static Stack<Ponto> caminho = new Stack<Ponto>();

        private static void Main(string[] args)
        {
            GerarLabirinto();
            DefinirPosicoesIniciais();
            ImprimirLabirinto();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Você desistiu. Boa sorte da próxima vez!");
                    break;
                }
                MoverRato(key.Key);
                ImprimirLabirinto();
                if (rato.Equals(queijo))
                {
                    Console.WriteLine($"Queijo encontrado na posição ({queijo.X + 1}, {queijo.Y + 1})!");
                    break;
                }
                if (!CaminhoPossivel())
                {
                    Console.WriteLine("Não é possível encontrar o queijo. Todas as direções levam a becos sem saída.");
                    break;
                }
            }
        }

        private static void GerarLabirinto()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    labirinto[i, j] = rand.NextDouble() < ProbabilidadeParede ? '|' : '.';
                }
            }
        }

        private static void DefinirPosicoesIniciais()
        {
            rato = new Ponto(rand.Next(N), rand.Next(N));
            queijo = new Ponto(rand.Next(N), rand.Next(N));
            labirinto[rato.X, rato.Y] = 'R';
            labirinto[queijo.X, queijo.Y] = 'Q';
        }

        private static void MoverRato(ConsoleKey key)
        {
            int newX = rato.X;
            int newY = rato.Y;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    newX--;
                    break;
                case ConsoleKey.DownArrow:
                    newX++;
                    break;
                case ConsoleKey.LeftArrow:
                    newY--;
                    break;
                case ConsoleKey.RightArrow:
                    newY++;
                    break;
                default:
                    return;
            }

            if (EstaNoLabirinto(newX, newY) && labirinto[newX, newY] != '|' && labirinto[newX, newY] != 'x')
            {
                caminho.Push(new Ponto(rato.X, rato.Y));
                labirinto[rato.X, rato.Y] = '.';
                rato.X = newX;
                rato.Y = newY;
                labirinto[rato.X, rato.Y] = 'R';
            }
            else if (EstaNoLabirinto(newX, newY))
            {
                if (labirinto[newX, newY] == '|')
                {
                    Retroceder();
                    labirinto[newX, newY] = 'x'; // Marca o beco sem saída com "x"
                }
                else if (labirinto[newX, newY] == 'x')
                {
                    Retroceder();
                }
            }
        }


        private static bool EstaNoLabirinto(int x, int y)
        {
            return x >= 0 && x < N && y >= 0 && y < N;
        }

        private static void Retroceder()
        {
            if (caminho.Count > 0)
            {
                Ponto ultimoMovimento = caminho.Pop();
                rato.X = ultimoMovimento.X;
                rato.Y = ultimoMovimento.Y;
                labirinto[rato.X, rato.Y] = 'R';
            }
        }

        private static bool CaminhoPossivel()
        {
            // Verifica se há pelo menos uma direção possível para movimentação
            int[] deltaX = { -1, 1, 0, 0 }; // Movimento para cima e para baixo
            int[] deltaY = { 0, 0, -1, 1 }; // Movimento para a esquerda e para a direita

            foreach (var i in deltaX)
            {
                foreach (var j in deltaY)
                {
                    int newX = rato.X + i;
                    int newY = rato.Y + j;
                    if (EstaNoLabirinto(newX, newY) && labirinto[newX, newY] != '|' && labirinto[newX, newY] != 'x' && !caminho.Contains(new Ponto(newX, newY)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void ImprimirLabirinto()
        {
            Console.Clear();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (caminho.Contains(new Ponto(i, j)))
                    {
                        Console.Write("R "); // Marca o caminho percorrido pelo rato com "R"
                    }
                    else
                    {
                        Console.Write(labirinto[i, j] + " ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    internal class Ponto
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Ponto(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Ponto ponto = (Ponto)obj;
            return X == ponto.X && Y == ponto.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}