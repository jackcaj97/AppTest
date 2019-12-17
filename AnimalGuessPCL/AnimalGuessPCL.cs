using System;
using System.Threading.Tasks;


namespace AnimalGuessPCL
{
    // Interfaccia IAuthenticate
    public interface IAuthenticate
    {
        Task<bool> Authenticate();
    }

    public class AnimalGuessPCL
    {
        public static IAuthenticate Authenticator { get; private set; }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

    }
}
