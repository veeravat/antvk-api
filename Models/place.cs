using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace antvk_api.Model
{
    public class place
    {
        [Key]
        public int placeID { get; set; }
        public string placeName { get; set; }
        public string placeDescription { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string placeImage { get; set; }
        public int placeType { get; set; }
        public List<events> events { get; set; }
    }

    public class events
    {
        [Key]
        public int eventID { get; set; }
        public string eventName { get; set; }
        public string eventDesc { get; set; }
        public DateTime eventDate { get; set; }
        public string eventImage { get; set; }
        public place place { get; set; }
    }
}