
// NOTE: This code will not compile obviously
public class LinqToJsonTests
{
    public void LinqToJsonBasic()
    {
        // start code LinqToJsonBasic
        JObject o = JObject.Parse(@"{
        'CPU': 'Intel',
        'Drives': [
          'DVD read/writer',
          '500 gigabyte hard drive' ]
        }");
        // end code LinqToJsonBasic
    }
}