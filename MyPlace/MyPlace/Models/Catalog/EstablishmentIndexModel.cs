﻿
namespace MyPlace.Models.Catalog
{
    using System.Collections.Generic;
    using MyPlace.DataModels;

    public class EstablishmentIndexModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string LocationLongitude { get; set; }

        public string LocationLatitude { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}

