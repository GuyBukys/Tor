using Bogus;
using System.Runtime.Serialization;

namespace Tor.TestsInfra;

internal class RecordFaker<T> : Faker<T> where T : class
{
    public RecordFaker()
    {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable SYSLIB0050
        this.CustomInstantiator(_ => FormatterServices.GetUninitializedObject(typeof(T)) as T);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore SYSLIB0050
    }
}
