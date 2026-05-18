namespace CryptoTrade
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. On lance d'abord le Login
            LoginForm login = new LoginForm();

            if (login.ShowDialog() == DialogResult.OK)
            {
                // 2. On passe l'ID récupéré au Form1
                Application.Run(new Form1(login.UserId));
            }
        }
    }
}