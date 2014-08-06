
// NOTE: This code will not compile obviously
public class LinqToJsonTests
{
    public void LinqToJsonBasic()
    {
        // startcode LinqToJsonBasic
        JObject o = JObject.Parse(@"{
        'CPU': 'Intel',
        'Drives': [
          'DVD read/writer',
          '500 gigabyte hard drive' ]
        }");
        // endcode 
    }
}