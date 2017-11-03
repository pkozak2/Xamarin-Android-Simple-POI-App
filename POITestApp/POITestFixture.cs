using System;
using System.Collections.Generic;
using NUnit.Framework;
using XamarinAndroidPoiApp.Managers;
using XamarinAndroidPoiApp.Models;


namespace POITestApp
{
    [TestFixture]
    public class POITestsFixture
    {

        [SetUp]
        public void Setup()
        {
            DbManager.Instance.CreateTable();
        }

        [Test]
        public void CreatePOI()
        {
            int testId = 1091;
            PointOfInterest newPOI = new PointOfInterest();
            newPOI.Id = testId;
            newPOI.Name = "New POI";
            newPOI.Description = "POI to test creating a new POI";
            newPOI.Address = "100 Main Street\nAnywhere, TX 75069";
            //Saving poi record  
            int recordsUpdated = DbManager.Instance.SavePOI(newPOI);
            //Check if the number of records updated are same as expected 
            Assert.AreEqual(1, recordsUpdated);
            // verify if the newly create POI exists  
            PointOfInterest poi = DbManager.Instance.GetPOI(testId);
            Assert.NotNull(poi);
            Assert.AreEqual(poi.Name, "New POI");
        }

        [Test]
        public void DeletePOI()
        {
            int testId = 1019;
            PointOfInterest testPOI = new PointOfInterest();
            testPOI.Id = testId;
            testPOI.Name = "Delete POI";
            testPOI.Description = "POI being saved so we can test delete";
            testPOI.Address = "100 Main Street\nAnywhere, TX 75069";
            DbManager.Instance.SavePOI(testPOI);
            PointOfInterest deletePOI = DbManager.Instance.GetPOI(testId);
            Assert.NotNull(deletePOI);
            DbManager.Instance.DeletePOI(testId);
            PointOfInterest poi = DbManager.Instance.GetPOI(testId);
            Assert.Null(poi);
        }

        [Test]
        public void ClearCache()
        {
            DbManager.Instance.ClearPOICache();
            List<PointOfInterest> poiList = DbManager.Instance.GetPOIListFromCache();
            Assert.AreEqual(0, poiList.Count);
        }


    }
}