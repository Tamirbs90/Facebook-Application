using System.Collections.Generic;
using System.Collections;
using System.Linq;
using FacebookWrapper.ObjectModel;

namespace AppLogic
{
    public class FirstLikersBuilder : IFeatureBuilder, IEnumerable, IEnumerable<UserProxy>
    {
        public User LoggedInUser { get; set; }
        private List<UserProxy> m_FirstLikersToPicsList = new List<UserProxy>();

        public void BuildFeature()
        {
            List<UserProxy> tempFirstLikersToPicsList = new List<UserProxy>();
            foreach (Photo photo in LoggedInUser.PhotosTaggedIn)
            {
                if (photo.LikedBy.Count > 0)
                {
                    User user = photo.LikedBy.First();
                    new UserProxy() { RealUser = user, NumOfLikes = 1 }.
                         AddUserToList(tempFirstLikersToPicsList);
                }
            }

            foreach (UserProxy user in tempFirstLikersToPicsList.
                OrderByDescending(key=>key.NumOfLikes))
            {
                if (user.NumOfLikes == tempFirstLikersToPicsList.First().NumOfLikes)
                {
                    m_FirstLikersToPicsList.Add(user);
                }
                else
                {
                    break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FirstLikersIterator(this);
        }

        IEnumerator<UserProxy> IEnumerable<UserProxy>.GetEnumerator()
        {
            foreach(UserProxy user in m_FirstLikersToPicsList)
            {
                yield return user;
            }
        }

        private class FirstLikersIterator : IEnumerator
        {
            private FirstLikersBuilder m_Collection;
            private int m_CurrentIdx = -1;
            private int m_Count = -1;

            public FirstLikersIterator(FirstLikersBuilder i_Collection)
            {
                m_Collection = i_Collection;
                m_Count = m_Collection.m_FirstLikersToPicsList.Count;
            }

            public object Current
            {
                get
                {
                    return m_Collection.m_FirstLikersToPicsList[m_CurrentIdx];
                }
            }

            public bool MoveNext()
            {
                ++m_CurrentIdx;
                return m_CurrentIdx < m_Collection.m_FirstLikersToPicsList.Count;
            }

            public void Reset()
            {
                m_CurrentIdx = 0;
            }
        }
    }
}

