using System.Collections.Generic;
using System.Collections;
using System.Linq;
using FacebookWrapper.ObjectModel;

namespace AppLogic
{
    public class LikesPerFriendBuilder : IFeatureBuilder, IEnumerable, IEnumerable<UserProxy>
    {
        public User LoggedInUser { get; set; }
        private List<UserProxy> m_LikesPerFriendList = new List<UserProxy>();

        public void BuildFeature()
        {
            List<UserProxy> tempLikesPerFriendList = new List<UserProxy>();
            foreach (Post post in LoggedInUser.Posts)
            {
                if (post.LikedBy.Count > 0)
                {
                    foreach (User friend in post.LikedBy)
                    {
                        new UserProxy() { RealUser = friend, NumOfLikes = 1 }.
                        AddUserToList(tempLikesPerFriendList);
                    }
                }
            }

            foreach (UserProxy user in tempLikesPerFriendList.
             OrderByDescending(key => key.NumOfLikes))
            {
                m_LikesPerFriendList.Add(user);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LikesPerFriendIterator(this);
        }

        IEnumerator<UserProxy> IEnumerable<UserProxy>.GetEnumerator()
        {
            foreach (UserProxy user in m_LikesPerFriendList)
            {
                yield return user;
            }
        }

        private class LikesPerFriendIterator : IEnumerator
        {
            private LikesPerFriendBuilder m_Collection;
            private int m_CurrentIdx = -1;
            private int m_Count = -1;

            public LikesPerFriendIterator(LikesPerFriendBuilder i_Collection)
            {
                m_Collection = i_Collection;
                m_Count = m_Collection.m_LikesPerFriendList.Count;
            }

            public object Current
            {
                get
                {
                    return m_Collection.m_LikesPerFriendList[m_CurrentIdx];
                }
            }

            public bool MoveNext()
            {
                ++m_CurrentIdx;
                return m_CurrentIdx < m_Collection.m_LikesPerFriendList.Count;
            }

            public void Reset()
            {
                m_CurrentIdx = 0;
            }
        }
    }
}

