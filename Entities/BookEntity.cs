

namespace library.Entities;
public class BookEntity {
  public int Id {get;set;}
  public required string Title {get;set;}
  public int AuthorId {get;set;}
  public AuthorEntity? Author {get;set;}
}
