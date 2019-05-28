using System;
using MongoDB.Bson; ///Utilise pour le ObjectId
using System.Collections.Generic;

namespace Tests
{
    //////////////////////////////////////////////////////////////////////////////////////
    /// Plants are mainly multicellular, predominantly photosynthetic eukaryotes of the kingdom Plantae.
    /// Historically, plants were treated as one of two kingdoms including all living things
    /// that were not animals, and all algae and fungi were treated as plants.
    /// However, all current definitions of Plantae exclude the fungi and some algae,
    /// as well as the prokaryotes (the archaea and bacteria). By one definition,
    /// plants form the clade Viridiplantae (Latin name for "green plants"), a group
    /// that includes the flowering plants, conifers and other gymnosperms, ferns and their allies,
    /// hornworts, liverworts, mosses and the green algae, but excludes the red and brown algae.
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Plant
    {
        //ID of the summary used in MongoDB.
        public ObjectId _id { get; set; }
        //Name of the plant.
        public string name { get; set; }
        //Light exposure needed by the plant.
        public string exposition { get; set; }
        //Aesthetics of the plant's leaves.
        public string port { get; set; }
        //Height of the plant, in centimeter.
        public int hauteur { get; set; }
        //Kind of soil the plant needs.
        public string substrat { get; set; }
        //Link to the picture.
        public string picture { get; set; }
    }
}