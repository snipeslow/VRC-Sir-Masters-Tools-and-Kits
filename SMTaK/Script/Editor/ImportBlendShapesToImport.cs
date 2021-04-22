using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportBlendShapesToImport : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (EditorPrefs.GetBool("SMTaKForceDefaultBlendShapeNormalsToImport", true))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.importBlendShapeNormals = ModelImporterNormals.Import;
        }
    }
}
