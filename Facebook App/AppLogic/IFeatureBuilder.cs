using FacebookWrapper.ObjectModel;

namespace AppLogic
{
    public interface IFeatureBuilder
    {
        User LoggedInUser { get; set; }
        void BuildFeature();
    }
}
