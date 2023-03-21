using FaceAmazon.Rekognition;

SearchFacesMatchingImage searchFaces = new SearchFacesMatchingImage();
Console.WriteLine("enter path");
string path = Console.ReadLine();

FileInfo info = new FileInfo(path);
string name = info.Name;

string truePath = "C:/Users/Admin/source/repos/FaceAmazon.Rekognition/" + name + ".txt";

File.WriteAllText(truePath, searchFaces.Example(path));