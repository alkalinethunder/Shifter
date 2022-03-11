using System;
using System.Collections;
using System.IO;
using Customization.User;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Customization.ShiftOS
{
    public abstract class ShiftOSImporter
    {
        private bool _wasSuccess = true;
        private Exception _exception = null;
        private string _destinationPath;

        public bool WasSuccessful => _wasSuccess;
        public Exception Exception => _exception;

        protected string SkinDestination => _destinationPath;
        
        protected ShiftOSImporter(string destinationPath)
        {
            Assert.IsTrue(Directory.Exists(destinationPath));
            _destinationPath = destinationPath;
        }

        public IEnumerator DoImport(Func<IEnumerator, Coroutine> startCoroutine)
        {
            _exception = null;
            yield return startCoroutine(LoadData());
            if (!_wasSuccess)
            {
                _exception ??= new Exception("Could not load skin data.");
                yield break;
            }

            _exception = null;
            yield return startCoroutine(CopyImages());
            if (!_wasSuccess)
            {
                _exception ??= new Exception("Could not copy all images.");
                yield break;
            }

            _exception = null;
            var userSkin = new UserSkin();
            yield return startCoroutine(BuildUserSkin(userSkin));
            if (!_wasSuccess)
            {
                _exception ??= new Exception("Could not build Socially Distant user skin.");
                yield break;
            }

            userSkin.Metadata.WasImportedFromShiftOS = true;

            FinalizeSkin(userSkin);
            _wasSuccess = true;
        }

        protected abstract IEnumerator LoadData();
        protected abstract IEnumerator CopyImages();
        protected abstract IEnumerator BuildUserSkin(UserSkin skin);
        
        private void FinalizeSkin(UserSkin data)
        {
            data.Metadata.About =
                "This skin was generated from a ShiftOS 0.0.8 skn file. Feel free to make further edits using the Content Manager, and to give it a proper name and author!";
            data.Metadata.Author = "ShiftOS Importer";
            data.Metadata.Name = Path.GetFileName(_destinationPath);

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var jsonPath = Path.Combine(_destinationPath, CustomizationSystem.StandardMetadataFileName);
            
            File.WriteAllText(jsonPath, json);
        }

        protected void ReportError(string message)
        {
            _wasSuccess = false;
            _exception = new Exception(message);
        }
    }
}