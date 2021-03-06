// Copyright (C) Jan-Willem Goossens 
// This code is licensed under the terms of the Eclipse Public License (EPL).

using System;
using System.Collections.Generic;

namespace Sonnet
{
    /// <summary>
    /// The class Named is a base class for entities that have a Name and 
    /// are Compared using an integer ID. 
    /// The ID must be set within the derived constructor.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4035:Classes implementing \"IEquatable<T>\" should be sealed", Justification = "By design")]
    public class Named : IComparable<Named>, IEquatable<Named>
    {
        /// <summary>
        /// Constructor of a new Named object with the given name.
        /// Empty string is used if no name is provided.
        /// </summary>
        /// <param name="name">The name is the new object, or empty string is not provided.</param>
        public Named(string name = null)
        {
            if (name != null) Name = name;
            else Name = "";
        }

        /// <summary>
        /// Gets or sets the name of this object.
        /// Name must be not-null.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                Ensure.NotNull(value, "name");
                this.name = value;
            }
        }

        /// <summary>
        /// Returns the ID of this object.
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Compares this object to the given object and returns true iff the objects are of the same type, and have the same ID.
        /// </summary>
        /// <param name="obj">The object to compare this to.</param>
        /// <returns>True iff the objects are of the same type, and have the same ID.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Named);
        }

        /// <summary>
        /// Compares this object to the given object and returns true iff the objects are of the same type, and have the same ID.
        /// </summary>
        /// <param name="other">The object to compare this to.</param>
        /// <returns>True iff the objects are of the same type, and have the same ID.</returns>
        public bool Equals(Named other)
        {
            if (other is null) return false;
            return id.Equals(other.id) && this.GetType().Equals(other.GetType());
        }

        /// <summary>
        /// Returns the ID of this object as hash code.
        /// </summary>
        /// <returns>A hash code (ID) for the current object.</returns>
        public override int GetHashCode()
        {
            // Find a way to make id readonly, because gethashcode should not be able
            // to change over the lifetime of an object.
            return id;
        }

        /// <summary>
        /// Compares the ID of this object to the ID of the given object.
        /// </summary>
        /// <param name="other">The object to compare this to.</param>
        /// <returns>The int.CompareTo value.</returns>
        public virtual int CompareTo(Named other)
        {
            if (other is null) return 1;
            return id.CompareTo(other.id);
        }

        //if we also implement the operators ==, !=, <, etc then those get called for Solver etc. while this not necessary

        private string name = string.Empty;
        internal int id;
    }

    /// <summary>
    /// The class ModelEntity is a base class for entities that are registered with solvers.
    /// Every ModelEntity can be Registered with multiple solvers, but can only be Assigned to at most one solver
    /// </summary>
	public class ModelEntity : Named
	{
        private readonly static SonnetLog log = SonnetLog.Default;

        /// <summary>
        /// Constructs a new ModelEntity with the given name.
        /// </summary>
        /// <param name="name">The name of this entity.</param>
        public ModelEntity(string name = null)
            : base(name)
        {
            solver = null;
        }

        /// <summary>
        /// Register the given solver with this entity.
        /// Whenever changes are made to this entity, these will be passed on to registerd solvers,
        /// such that the COIN solver can be updated.
        /// Therefore, only Generated entities (passed to the COIN solver) need to be registered.
        /// </summary>
        /// <param name="solver">The solver of which this constraint is part.</param>
        internal void Register(Solver solver)
        {
            if (IsRegistered(solver))
            {
                string message = log.ErrorFormat("A solver can be registered at most once, even for {1} '{0}'!", this.Name, this.GetType().Name);
                throw new SonnetException(message);
            }

            solvers.Add(solver);
        }

        /// <summary>
        /// Remove the given solver from the list of registered solvers.
        /// If the solver was not registered, an exception will be thrown.
        /// </summary>
        /// <param name="solver">The solver</param>
        internal void Unregister(Solver solver)
        {
            if (!solvers.Remove(solver))
            {
                string message = log.ErrorFormat("Solver {0} cannot be unregisterd because it is not registered with {1} '{2}'!", solver.Name, this.GetType().Name, this.Name);
                throw new SonnetException(message);
            }

            if (AssignedTo(solver))
            {
                Unassign();
            }
        }

        /// <summary>
        /// Returns true iff the given solver is registered with this entity.
        /// </summary>
        /// <param name="solver">The solver</param>
        /// <returns>True iff the given solver is registered with this entity.</returns>
        internal bool IsRegistered(Solver solver)
        {
            // first, if this modelEntity is Assigned 
            // ( = has an assigned model which is thus also registered by definition),
            // then simply check if this assigned model is the one requested.
            if (Assigned && object.ReferenceEquals(this.solver, solver)) return true;

            int id = solver.ID;
           // return solvers.Find(s => s == solver);
            return solvers.Find(s => string.Equals(s.ID, id)) != null;
        }

        /// <summary>
        /// Set the given solver to be the assigned solver, where this entity has the given offset.
        /// </summary>
        /// <param name="solver">The newly assigned solver.</param>
        /// <param name="offset">The offset of the current entity in the solver.</param>
        internal void Assign(Solver solver, int offset)
        {
            // if this modelEntity is already assigned, 
            // then if it is assigned to another model throw an exception
            //      or if it has a different offset within the requested model, throw an exception
            if (Assigned && (!AssignedTo(solver) || offset != this.offset))
            {
                string message = log.ErrorFormat("Trying to assign to new solver while already assigned to solver ", solver.Name);
                throw new SonnetException(message);
            }

            this.solver = solver;
            this.offset = offset;
        }

        private void Unassign()
        {
            solver = null;
            offset = -1;
        }

        /// <summary>
        /// Gets the offset of this entity in the assigned solver.
        /// </summary>
        internal int Offset
        {
            get { return offset; }
        }

        /// <summary>
        /// Determines whether the given solver is the current Assigned solver.
        /// </summary>
        /// <param name="solver">The solver to compare.</param>
        /// <returns>True iff the given solver is the same as the current solver.</returns>
        public bool AssignedTo(Solver solver)
        {
            return this.solver == solver;
        }

        /// <summary>
        /// Gets the currently assigned solver.
        /// </summary>
        internal Solver AssignedSolver
        {
            get { return solver; }
        }

        /// <summary>
        /// Determines whether this entity is assigned to a solver.
        /// </summary>
        internal bool Assigned
        {
            get { return !(solver is null); }
        }

        /// <summary>
        /// The list of solvers with which this entity is registered.
        /// Derived classes use this to notify solvers of any changes to them (name, bounds, etc)
        /// </summary>
		protected List<Solver> solvers = new List<Solver>();
        private Solver solver = null;
		private int offset = -1;
	}
}
