namespace ToileDeFond.Website.Administration.Models
{
    public class EmbeddedContentPropertyEditorViewModel : PropertyEditorViewModelBase
    {
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }

        //TODO: Ajouter du metadata pour dire quel type de contenu etc..
    }
}