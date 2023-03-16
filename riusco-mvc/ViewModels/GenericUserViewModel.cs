namespace riusco_mvc.ViewModels
{
    public class GenericUserViewModel
    {
        public string Email { get; set; }
        public string Outcome { get; set; }

        public GenericUserViewModel(string email)
        {
            Email = email;
        }
        
        public GenericUserViewModel(string email, string outcome)
        {
            Email = email;
            Outcome = outcome;
        }
    }
}