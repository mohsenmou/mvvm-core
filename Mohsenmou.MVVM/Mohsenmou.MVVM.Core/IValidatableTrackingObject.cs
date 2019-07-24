using System.ComponentModel;

namespace Mohsenmou.MVVM.Core
{
    public interface IValidatableTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
    {
        bool IsValid { get; }
    }
}
