using System.Text.Json;

namespace Seed
{
    static class Program
    {

        private static ProxyType[]? proxies;

        static List<SeedQuery>? LoadQuery()
        {
            try
            {
                var contents = File.ReadAllText(@"data.txt");
                return JsonSerializer.Deserialize<List<SeedQuery>>(contents);
            }
            catch { }

            return null;
        }

        static ProxyType[]? LoadProxy()
        {
            try
            {
                var contents = File.ReadAllText(@"proxy.txt");
                return JsonSerializer.Deserialize<ProxyType[]>(contents);
            }
            catch { }

            return null;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("  ____                _ ____   ___ _____ \r\n / ___|  ___  ___  __| | __ ) / _ \\_   _|\r\n \\___ \\ / _ \\/ _ \\/ _` |  _ \\| | | || |  \r\n  ___) |  __/  __/ (_| | |_) | |_| || |  \r\n |____/ \\___|\\___|\\__,_|____/ \\___/ |_|  \r\n                                         ");
            Console.WriteLine();
            Console.WriteLine("Github: https://github.com/glad-tidings/SeedBot");
            Console.WriteLine();
            Console.Write("Select an option:\n1. Run bot\n2. Create session\n> ");
            string? opt = Console.ReadLine();

            var SeedQueries = LoadQuery();
            proxies = LoadProxy();

            if (opt != null)
            {
                if (opt == "1")
                {
                    foreach (var Query in SeedQueries ?? [])
                    {
                        var BotThread = new Thread(() => SeedThread(Query)); BotThread.Start();
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var Query in SeedQueries ?? [])
                    {
                        if (!File.Exists(@$"sessions\{Query.Name}.session"))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})");
                            TelegramMiniApp.WebView vw = new(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "");
                            if (vw.Save_Session().Result)
                                Console.WriteLine("Session created");
                            else
                                Console.WriteLine("Create session failed");
                        }
                    }

                    Environment.Exit(0);
                }
            }
        }

        public async static void SeedThread(SeedQuery Query)
        {
            while (true)
            {
                var RND = new Random();

                try
                {
                    var Bot = new SeedBot(Query, proxies ?? []);
                    if (!Bot.HasError)
                    {
                        Log.Show("Seed", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White);
                        Query = Bot.PubQuery;
                        Log.Show("Seed", Query.Name, $"login successfully.", ConsoleColor.Green);
                        var Sync = await Bot.SeedGetBalance();
                        if (Sync is not null)
                        {
                            Log.Show("Seed", Query.Name, $"synced successfully. B<{((double)Sync.Data / 1000000000d).ToString("N2")}>", ConsoleColor.Blue);
                            if (Query.DailyReward)
                            {
                                var dailyReward = await Bot.SeedDailyReward();
                                if (dailyReward is not null)
                                {
                                    if (dailyReward.Data is not null)
                                    {
                                        if (dailyReward.Data.Count > 0)
                                        {
                                            var streaks = new List<string>();
                                            foreach (var rew in dailyReward.Data)
                                                streaks.Add(rew.Id);
                                            bool reward = await Bot.SeedClaimDailyReward(streaks);
                                            if (reward)
                                                Log.Show("Seed", Query.Name, $"daily reward claimed", ConsoleColor.Green);
                                            else
                                                Log.Show("Seed", Query.Name, $"claim daily reward failed", ConsoleColor.Red);

                                            Thread.Sleep(3000);
                                        }
                                    }
                                }
                            }

                            if (Query.Seed)
                            {
                                if (Bot.UserDetail.Data?.LastClaim.ToLocalTime().AddHours(1d) < DateTime.Now)
                                {
                                    bool claimSeed = await Bot.SeedClaimSeed();
                                    if (claimSeed)
                                        Log.Show("Seed", Query.Name, $"seed claimed", ConsoleColor.Green);
                                    else
                                        Log.Show("Seed", Query.Name, $"claim seed failed", ConsoleColor.Red);

                                    Thread.Sleep(3000);
                                }
                            }

                            if (Query.Worm)
                            {
                                var worms = await Bot.SeedWorms();
                                if (worms is not null)
                                {
                                    if (worms.Data is not null)
                                    {
                                        if (worms.Data.CreatedAt.ToLocalTime() < DateTime.Now & !worms.Data.IsCaught)
                                        {
                                            bool worm = await Bot.SeedClaimWorm();
                                            if (worm)
                                                Log.Show("Seed", Query.Name, $"worm caught", ConsoleColor.Green);
                                            else
                                                Log.Show("Seed", Query.Name, $"catch worm failed", ConsoleColor.Red);

                                            Thread.Sleep(3000);
                                        }
                                    }
                                }
                            }

                            if (Query.Egg & !Bot.UserDetail.Data.GiveFirstEgg)
                            {
                                bool egg = await Bot.SeedClaimEgg();
                                if (egg)
                                    Log.Show("Seed", Query.Name, $"first egg given", ConsoleColor.Green);
                                else
                                    Log.Show("Seed", Query.Name, $"give first egg failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }

                            if (Query.Task)
                            {
                                var tasks = await Bot.SeedTasks();
                                if (tasks is not null)
                                {
                                    foreach (var task in tasks.Data?.Where(x => x.Type == "Join community" | x.Type == "Follow us" | x.Type == "TG story" | x.Type == "Play app") ?? [])
                                    {
                                        if (task.TaskUser is null)
                                        {
                                            var doneTask = Bot.SeedDoneTask(task.Id);
                                            if (doneTask is not null)
                                                Log.Show("Seed", Query.Name, $"task '{task.Name}' started", ConsoleColor.Green);
                                            else
                                                Log.Show("Seed", Query.Name, $"start task '{task.Name}' failed", ConsoleColor.Red);

                                            int eachtaskRND = RND.Next(Query.TaskSleep[0], Query.TaskSleep[1]);
                                            Thread.Sleep(eachtaskRND * 1000);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            Log.Show("Seed", Query.Name, $"synced failed", ConsoleColor.Red);

                        Sync = await Bot.SeedGetBalance();
                        if (Sync is not null)
                            Log.Show("Seed", Query.Name, $"B<{((double)Sync.Data / 1000000000d).ToString("N2")}>", ConsoleColor.Blue);
                    }
                    else
                        Log.Show("Seed", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Log.Show("Seed", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red);
                }

                int syncRND = 0;
                if (DateTime.Now.Hour < 8)
                    syncRND = RND.Next(Query.NightSleep[0], Query.NightSleep[1]);
                else
                    syncRND = RND.Next(Query.DaySleep[0], Query.DaySleep[1]);
                Log.Show("Seed", Query.Name, $"sync sleep '{Convert.ToInt32(syncRND / 3600d)}h {Convert.ToInt32(syncRND % 3600 / 60d)}m {syncRND % 60}s'", ConsoleColor.Yellow);
                Thread.Sleep(syncRND * 1000);
            }
        }
    }
}