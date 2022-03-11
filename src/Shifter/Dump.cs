        /*
        #region ShiftOS Support
        
        public void ImportShiftOSSkin(string path)
        {
            Debug.Log("Attempting to import ShiftOS Skin: " + path, this);

            StartCoroutine(ExtractShiftOSSkin(path));
        }

        private IEnumerator ExtractShiftOSSkin(string path)
        {
            var format = IdentifyShiftOSFormat(path);
            yield return null;

            if (format == KnownShiftOSFormat.Unknown)
            {
                Debug.LogError("That isn't a ShiftOS skin, or, it is, but we have no fucking clue how to read it.");
                yield break;
            }
            
            Debug.Log($"Identified possible format: {format}", this);
            
            // We still do not know the format of the skin. For example, just because
            // it's a ZIP file (Phil), doesn't mean it has Phil's data blob in it. It might have
            // a JSON data blob, which makes it a Michael2016 format skin.
            // Still, we should prepare the skin for importing.

            // Folder name for the new Socially Distant skin. Append the current time so we don't get conflicts.
            var skinName = Path.GetFileNameWithoutExtension(path) + $"-{DateTime.UtcNow.ToFileTime()}";
            var destPath = Path.Combine(skinsPath, skinName);
            Debug.Log("Creating new Socially Distant Skin at path: " + destPath);
            Directory.CreateDirectory(destPath);
            yield return null;
            
            // Now, we should create a temp path for the resources we're about to import.
            // For ZIP skins, this is where they'll be extracted.
            var skinTemp = Path.Combine(destPath, ".TEMP");
            Directory.CreateDirectory(skinTemp);
            yield return null;
            
            // Extract the skin, or just move the JSON.
            if (format == KnownShiftOSFormat.Michael2017)
            {
                File.Copy(path, Path.Combine(skinTemp, "michael.json"));
            }
            else
            {
                ZipFile.ExtractToDirectory(path, skinTemp);
            }

            Debug.Log("Moved ShiftOS data to " + skinTemp);
            yield return null;
            
            // Identify the subformat.
            if (File.Exists(Path.Combine(skinTemp, "data.dat")))
            {
                format = KnownShiftOSFormat.PhilNew;
                Debug.Log("Identified skin sub-format: Phil (new)");
            }
            else if (File.Exists(Path.Combine(skinTemp, "skindata.dat")))
            {
                Debug.Log("Identified a ShiftOS 0.0.7 skin.");
            }
            else if (File.Exists(Path.Combine(skinTemp, "data.json")))
            {
                format = KnownShiftOSFormat.Michael2016;
                Debug.Log("Identified a ShiftOS-C# skin.");
            }
            else if (File.Exists(Path.Combine(skinTemp, "michael.json")))
            {
                Debug.Log("Identified a ShiftOS 1.0 skin.");
            }

            var importer = null as ShiftOSImporter;
            switch (format)
            {
                case KnownShiftOSFormat.Phil:
                    importer = new PhilImporter(skinTemp, destPath);
                    break;
                case KnownShiftOSFormat.PhilNew:
                    importer = new NewPhilImporter(skinTemp, destPath);
                    break;
                case KnownShiftOSFormat.Michael2016:
                    importer = new ShiftOSCSharpImporter(skinTemp, destPath);
                    break;
                case KnownShiftOSFormat.Michael2017:
                    importer = new OneShiftOSImporter(Path.Combine(skinTemp, "michael.json"), destPath);
                    break;
            }

            if (importer == null)
            {
                Debug.LogWarning("Import failed - could not reasonably determine the skin format.");
                Directory.Delete(destPath, true);
                yield break;
            }

            yield return StartCoroutine(importer.DoImport(StartCoroutine));

            if (!importer.WasSuccessful)
            {
                Debug.Log($"Import failed - {importer.Exception.Message}");
                Directory.Delete(destPath, true);
                yield break;
            }
            
            // Delete the temporary data.
            Directory.Delete(skinTemp, true);
            Debug.Log("Import successful!");

            yield return StartCoroutine(RefreshSkinDatabase());
        }

        private KnownShiftOSFormat IdentifyShiftOSFormat(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                // First we need to identify the skin file type.
                // We can be reasonably sure it's 0.0.x if we get a ZIP header.
                var zipBytes = new byte[] {0x50, 0x4B, 0x03, 0x04};
                var magicBuffer = new byte[zipBytes.Length];

                // Read the first 4 bytes of the file.
                stream.Read(magicBuffer, 0, magicBuffer.Length);
                stream.Position = 0;

                if (magicBuffer.SequenceEqual(zipBytes))
                    return KnownShiftOSFormat.Phil;
                
                // Try another read.
                var jsonBytes = Encoding.UTF8.GetBytes("{");
                magicBuffer = new byte[jsonBytes.Length];
                stream.Read(magicBuffer, 0, magicBuffer.Length);
                stream.Position = 0;
                if (jsonBytes.SequenceEqual(magicBuffer))
                {
                    Debug.Log($"Identified possible JSON data in {path}.");

                    // Read everything, we gotta parse this.
                    using var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();

                    try
                    {
                        var jobj = JObject.Parse(json);
                        return KnownShiftOSFormat.Michael2017;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Skin JSON wasn't parsed properly: {ex.Message} - Not a ShiftOS skin.", this);
                    }
                }

            }
            
            // If we haven't decided a format, then it's not known.
            return KnownShiftOSFormat.Unknown;
        }
        
        #endregion
        */
