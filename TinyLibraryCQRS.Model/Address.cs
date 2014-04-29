/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Model
{
    public class Address : IEquatable<Address>
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Address address = obj as Address;
            if (address == null)
                return false;
            return this.Equals(address);
        }

        public override int GetHashCode()
        {
            return this.Country.GetHashCode() ^
                this.State.GetHashCode() ^
                this.City.GetHashCode() ^
                this.Street.GetHashCode() ^
                this.Zip.GetHashCode();
        }

        #region IEquatable<Address> Members

        public bool Equals(Address other)
        {
            if (object.ReferenceEquals(this, other))
                return true;
            if ((object)other == null)
                return false;
            return this.Country.Equals(other.Country) &&
                this.City.Equals(other.City) &&
                this.State.Equals(other.State) &&
                this.Street.Equals(other.Street) &&
                this.Zip.Equals(other.Zip);
        }

        #endregion
    }
}
