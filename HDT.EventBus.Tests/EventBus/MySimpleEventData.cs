namespace HDT.EventBus.Tests.EventBus
{
    public class MySimpleEventData 
    {
        public int Value { get; set; }
        public MySimpleEventData(int value) 
        { 
            Value = value;
        }
    }
}
