using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Tipnet.Models;
using Tipnet.Repository;

namespace Tipnet.CustomAtributes
{
    public class CustomPrincipal : IPrincipal
    {
        private Player Player;

        public CustomPrincipal(Player player)
        {
            Player = player;
            Identity = new GenericIdentity(player.Username);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] {','});
            return roles.Any(r => Player.Role.Contains(r));
        }
    }
}