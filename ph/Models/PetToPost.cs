namespace ph.Models
{
    public class PetToPost
    {
        public string PetId { get; set; }
        public Pet Pet { get; set; }

        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}