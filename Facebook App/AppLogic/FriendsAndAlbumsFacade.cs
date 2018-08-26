using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FacebookWrapper.ObjectModel;

namespace AppLogic
{
     public class FriendsAndAlbumsFacade
     {
          private User m_LoggedInUser;
          private LinkedList<Album> m_Albums;
          private LinkedList<User> m_friends;
          public FriendsAndAlbumsFacade(User io_LoggedInUser)
          {
               m_LoggedInUser = io_LoggedInUser;
               m_Albums = new LinkedList<Album>();
               m_friends = new LinkedList<User>();
          }

          public LinkedList<Album> Albums
          {
               get { return m_Albums; }
          }
          public LinkedList<User> Friends
          {
               get { return m_friends; }
          }
          public void FetchAlbums()
          {
               new Thread(fetchAlbums).Start();
          }
          public void FetchFriends()
          {
               new Thread(fetchFriends).Start();
          }

          private void fetchAlbums()
          {
               foreach(var album in m_LoggedInUser.Albums)
               {
                    m_Albums.AddLast(album);
                    
               }
               
          }

          private void fetchFriends()
          {
               foreach(var friend in m_LoggedInUser.Friends)
               {
                    m_friends.AddLast(friend);
               }
               m_friends.OrderByDescending(key => key.Name);
               
          }



      
          
     }
}
