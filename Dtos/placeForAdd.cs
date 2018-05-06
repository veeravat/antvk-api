using System;
using System.ComponentModel.DataAnnotations;

namespace antvk_api.Dtos
{
    public class placeForAdd
    {
        [Required] 
        public string placeName { get; set; }

        [Required] 
        public string placeDescription { get; set; }
        [Required] 
        public string placeImageEXT { get; set; }

        [Required, Range(float.MinValue, float.MaxValue, ErrorMessage = "Please enter valid latitude")]
        public float latitude { get; set; }

        [Required, Range(float.MinValue, float.MaxValue, ErrorMessage = "Please enter valid longitude")]
        public float longitude { get; set; }

        [Required] 
        public byte[] placeImage { get; set; }

        [Required, Range(int.MinValue, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int placeType { get; set; }
    
    }
    public class eventForAdd
    {
        [Required] 
        public string eventName { get; set; }

        [Required] 
        public string eventDesc { get; set; }
        [Required] 
        public string eventImageEXT { get; set; }
        
        [Required] 
        public byte[] eventImage { get; set; }
        
        [Required, Range(int.MinValue, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int placeID { get; set; }
    
        [Required]     
        public DateTime eventDate { get; set; }
    }
}