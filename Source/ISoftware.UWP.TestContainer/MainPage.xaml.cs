using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ISoftware.UWP.TestContainer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            _viewModel = new ViewModel(Dispatcher);

            DataContext = _viewModel;
            Loaded += OnMainPageLoaded;
        }

        private async void OnMainPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _viewModel.Initialise();
        }

        private readonly ViewModel _viewModel;
    }
}
