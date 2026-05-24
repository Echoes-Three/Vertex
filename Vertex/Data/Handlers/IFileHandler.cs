namespace Vertex.Models.Contracts;

public interface IFileHandler<T>
{
    public void Save(T entry){}
    public void Delete(T entry){}
    public void Serialize(){}
    public void Load(){}
}