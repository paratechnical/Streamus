﻿using log4net;
using NHibernate;
using Streamus.Backend.Domain;
using Streamus.Backend.Domain.Interfaces;
using System;
using System.Reflection;

namespace Streamus.Backend.Dao
{
    public class PlaylistDao : AbstractNHibernateDao<Playlist>, IPlaylistDao
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Playlist Get(Guid id)
        {
            Playlist playlist = null;

            try
            {
                playlist = (Playlist)NHibernateSession.Load(typeof(Playlist), id);
            }
            catch (ObjectNotFoundException exception)
            {
                //  Consume error and return null.
                Logger.Error(exception);
            }

            return playlist;
        }
    }
}