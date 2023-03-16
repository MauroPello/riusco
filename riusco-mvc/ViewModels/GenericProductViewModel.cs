namespace riusco_mvc.ViewModels
{
    public class GenericProductViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Outcome { get; set; }

        public GenericProductViewModel(string name)
        {
            Name = name;
        }
        
        public GenericProductViewModel(int id)
        {
            Id = id;
        }
        
        public GenericProductViewModel(int id, string outcome)
        {
            Id = id;
            Outcome = outcome;
        }
        
        public GenericProductViewModel(string name, int id)
        {
            Id = id;
            Name = name;
        }
        
        public GenericProductViewModel(string name, string outcome)
        {
            Name = name;
            Outcome = outcome;
        }
        
        public GenericProductViewModel(string name, int id, string outcome)
        {
            Name = name;
            Id = id;
            Outcome = outcome;
        }
        public GenericProductViewModel(){}
    }
}