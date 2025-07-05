namespace MusicalMemory.CandleSystem
{
    public class RemainedCandleProperty
    {
        public int RemainedIndex { get; private set; }
        public int MaxCount { get; }
        public Candle SelectedCandle { get; set; }

        public RemainedCandleProperty(int _maxCount = 1)
        {
            MaxCount = _maxCount;
            RemainedIndex = 0;
        }

        internal void IncreaseRemainedCount(int _amount = 1) => RemainedIndex += _amount;
    }
}