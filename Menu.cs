using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crosses
{
    class Menu
    {

        string displayName;

        enum State { 
            MAIN,
            HOST,
            JOIN,
            ROOM,
            CONN_ERR
        }

        State state;
        bool running;

        public Menu() {
            state = State.MAIN;
        }

        public void Run() {
            running = true;
            while (running == true)
            {
                StateMachine();
            }
        }

        private void StateMachine(){
            switch (state) {
                case State.MAIN:
                    MainMenu();
                    break;
                case State.HOST:
                    HostMenu();
                    break;
                case State.JOIN:
                    NameMenu();
                    break;
                case State.ROOM:
                    ClientMenu();
                    break;
                default:
                    state = State.MAIN;
                    break;
            }
        }

        private void MainMenu() {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("1: Host new room");
            Console.WriteLine("2: Join room");
            Console.WriteLine("3: Quit");

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.D1)
                {
                    state = State.HOST;
                    return;
                }
                if (cki.Key == ConsoleKey.D2)
                {
                    state = State.JOIN;
                    return;
                }
                if (cki.Key == ConsoleKey.D3)
                {
                    running = false;
                    return;
                }
            }
        }

        private void HostMenu()
        {
            Console.Clear();

            Server server = new Server();
            server.Start();
            state = State.MAIN;

        }

        private void ClientMenu()
        {
            Console.Clear();

            Client client = new Client();
            client.ExecuteClient(displayName);
            state = State.MAIN;

        }

        private void NameMenu()
        {
            Console.Clear();
            Console.WriteLine("Enter your display name");
            displayName = Console.ReadLine();
            state = State.ROOM;
        }
    }
}
