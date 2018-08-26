using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace AppLogic
{
    public class UserProxy 
    {
        public User RealUser { get; set; }
        public int NumOfLikes { get; set; }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            UserProxy userToCompare = obj as UserProxy;
            if (userToCompare != null)
            {
                if (this.RealUser.Id.Equals(userToCompare.RealUser.Id))
                {
                    isEqual = true;
                }
            }

            return isEqual;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", RealUser.Name, NumOfLikes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AddUserToList(List<UserProxy> i_UsersList)
        {
            int userIndex = i_UsersList.IndexOf(this);

            if (userIndex == -1)
            {
                i_UsersList.Add(this);
            }
            else
            {
                i_UsersList[userIndex].NumOfLikes++;
            }
        }
    }
}
