﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using Streamus.Backend.Domain.Validators;

namespace Streamus.Backend.Domain
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        //  Use collection interfaces so NHibernate can inject with its own collection implementation.
        [DataMember(Name = "playlistCollections")]
        public IList<PlaylistCollection> PlaylistCollections { get; set; }

        public User()
        {
            Name = string.Empty;
            PlaylistCollections = new List<PlaylistCollection>();

            //  A user should always have at least one PlaylistCollection.
            CreatePlaylistCollection();
        }

        public PlaylistCollection CreatePlaylistCollection()
        {
            string title = string.Format("New Playlist Collection {0:D4}", PlaylistCollections.Count);
            var playlistCollection = new PlaylistCollection(title);
            return AddPlaylistCollection(playlistCollection);
        }

        public PlaylistCollection AddPlaylistCollection(PlaylistCollection playlistCollection)
        {
            playlistCollection.User = this;
            PlaylistCollections.Add(playlistCollection);

            return playlistCollection;
        }

        public void RemovePlaylistCollection(PlaylistCollection playlistCollection)
        {
            PlaylistCollections.Remove(playlistCollection);
        }

        public void ValidateAndThrow()
        {
            var validator = new UserValidator();
            validator.ValidateAndThrow(this);
        }

        private int? _oldHashCode;
        public override int GetHashCode()
        {
            // Once we have a hash code we'll never change it
            if (_oldHashCode.HasValue)
                return _oldHashCode.Value;

            bool thisIsTransient = Equals(Id, Guid.Empty);

            // When this instance is transient, we use the base GetHashCode()
            // and remember it, so an instance can NEVER change its hash code.
            if (thisIsTransient)
            {
                _oldHashCode = base.GetHashCode();
                return _oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            User other = obj as User;
            if (other == null)
                return false;

            // handle the case of comparing two NEW objects
            bool otherIsTransient = Equals(other.Id, Guid.Empty);
            bool thisIsTransient = Equals(Id, Guid.Empty);
            if (otherIsTransient && thisIsTransient)
                return ReferenceEquals(other, this);

            return other.Id.Equals(Id);
        }
    }
}