using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ETA
{
    private DateTime _startTime;
    private int _totalItems;
    private int _processedItems;
    public ETA(int totalItems)
    {
        _startTime = DateTime.Now;
        _totalItems = totalItems;
        _processedItems = 0;
    }
    public void Update(int processedItems)
    {
        _processedItems = processedItems;
    }
    public TimeSpan GetRemainingTime()
    {
        if (_processedItems == 0) return TimeSpan.MaxValue;
        var elapsed = DateTime.Now - _startTime;
        var estimatedTotal = elapsed.TotalSeconds / _processedItems * _totalItems;
        return TimeSpan.FromSeconds(estimatedTotal - elapsed.TotalSeconds);
    }
}
