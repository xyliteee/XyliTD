using System.Windows.Controls;

namespace XyliTDMain.Dynamic
{
    public class SingleCard
    {
        public Image MusicImage;
        public Label TitleLabel;
        public Label DescriptionLabel;
        public Label StateLabel;
        public ProgressBar ProgressBar;
        public Button DirButton;
        public Button FileButton;
        public Button ShowDialogButton;
        public SingleCard(Image musicImage,Label titleLabel,Label descriptionLabel,Label stateLabel,ProgressBar ProgressBar,Button dirbutton,Button fileButton,Button showDialogButton) 
        {
            MusicImage = musicImage;
            TitleLabel = titleLabel;
            DescriptionLabel = descriptionLabel;
            StateLabel = stateLabel;
            this.ProgressBar = ProgressBar;
            DirButton = dirbutton;
            FileButton = fileButton;
            ShowDialogButton = showDialogButton;
        }
    }
}
