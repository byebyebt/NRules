using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Extensibility;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents a rules engine session. Created by <see cref="ISessionFactory"/>.
    /// Each session has its own working memory, and exposes operations that 
    /// manipulate facts in it, as well as fire matching rules.
    /// </summary>
    /// <event cref="IEventProvider.FactInsertingEvent">Before processing fact insertion.</event>
    /// <event cref="IEventProvider.FactInsertedEvent">After processing fact insertion.</event>
    /// <event cref="IEventProvider.FactUpdatingEvent">Before processing fact update.</event>
    /// <event cref="IEventProvider.FactUpdatedEvent">After processing fact update.</event>
    /// <event cref="IEventProvider.FactRetractingEvent">Before processing fact retraction.</event>
    /// <event cref="IEventProvider.FactRetractedEvent">After processing fact retraction.</event>
    /// <event cref="IEventProvider.ActivationCreatedEvent">When a set of facts matches a rule.</event>
    /// <event cref="IEventProvider.ActivationUpdatedEvent">When a set of facts is updated and re-matches a rule.</event>
    /// <event cref="IEventProvider.ActivationDeletedEvent">When a set of facts no longer matches a rule.</event>
    /// <event cref="IEventProvider.RuleFiringEvent">Before rule's actions are executed.</event>
    /// <event cref="IEventProvider.RuleFiredEvent">After rule's actions are executed.</event>
    /// <event cref="IEventProvider.ConditionFailedEvent">When there is an error during condition evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.BindingFailedEvent">When there is an error during binding expression evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.AggregateFailedEvent">When there is an error during aggregate expression evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.AgendaFilterFailedEvent">When there is an error during agenda filter evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.ActionFailedEvent">When there is an error during action evaluation,
    /// before throwing exception to the client.</event>
    /// <exception cref="RuleConditionEvaluationException">Error while evaluating any of the rules' conditions.
    /// This exception can also be observed as an event <see cref="IEventProvider.ConditionFailedEvent"/>.</exception>
    /// <exception cref="RuleExpressionEvaluationException">Error while evaluating any of the rules' binding or aggregate expressions.
    /// This exception can also be observed as events <see cref="IEventProvider.BindingFailedEvent"/>, <see cref="IEventProvider.AggregateFailedEvent"/>.</exception>
    /// <exception cref="AgendaExpressionEvaluationException">Error while evaluating any of the agenda filters.
    /// This exception can also be observed as an event <see cref="IEventProvider.AgendaFilterFailedEvent"/>.</exception>
    /// <exception cref="RuleActionEvaluationException">Error while evaluating any of the rules' actions.
    /// This exception can also be observed as an event <see cref="IEventProvider.ActionFailedEvent"/>.</exception>
    /// <seealso cref="ISessionFactory"/>
    /// <threadsafety instance="false" />
    public interface ISession
    {
        /// <summary>
        /// Agenda, which represents a store for rule matches.
        /// </summary>
        IAgenda Agenda { get; }

        /// <summary>
        /// Provider of events from the current rule session.
        /// Use it to subscribe to various rules engine lifecycle events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Rules dependency resolver for the current rules session.
        /// </summary>
        IDependencyResolver DependencyResolver { get; set; }

        /// <summary>
        /// Action interceptor for the current rules session.
        /// If provided, invocation of rule actions is delegated to the interceptor.
        /// </summary>
        IActionInterceptor ActionInterceptor { get; set; }

        /// <summary>
        /// Inserts new facts to the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to insert.</param>
        /// <exception cref="ArgumentException">If any fact already exists in working memory.</exception>
        void InsertAll(IEnumerable<object> facts);

        /// <summary>
        /// Inserts new facts to the rules engine memory if the facts don't exist.
        /// If any of the facts exists in the engine, none of the facts are inserted.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to insert.</param>
        /// <returns>Result of facts insertion.</returns>
        IFactResult TryInsertAll(IEnumerable<object> facts);

        /// <summary>
        /// Inserts new fact to the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="fact">Facts to insert.</param>
        /// <exception cref="ArgumentException">If fact already exists in working memory.</exception>
        void Insert(object fact);

        /// <summary>
        /// Inserts a fact to the rules engine memory if the fact does not exist.
        /// </summary>
        /// <param name="fact">Fact to insert.</param>
        /// <returns>Whether the fact was inserted or not.</returns>
        bool TryInsert(object fact);

        /// <summary>
        /// Updates existing facts in the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to update.</param>
        /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
        void UpdateAll(IEnumerable<object> facts);

        /// <summary>
        /// Updates existing facts in the rules engine memory if the facts exist.
        /// If any of the facts don't exist in the engine, none of the facts are updated.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to update.</param>
        /// <returns>Result of facts update.</returns>
        IFactResult TryUpdateAll(IEnumerable<object> facts);

        /// <summary>
        /// Updates existing fact in the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="fact">Fact to update.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Update(object fact);

        /// <summary>
        /// Updates a fact in the rules engine memory if the fact exists.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="fact">Fact to update.</param>
        /// <returns>Whether the fact was updated or not.</returns>
        bool TryUpdate(object fact);

        /// <summary>
        /// Removes existing facts from the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to remove.</param>
        /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
        void RetractAll(IEnumerable<object> facts);

        /// <summary>
        /// Removes existing facts from the rules engine memory if the facts exist.
        /// If any of the facts don't exist in the engine, none of the facts are removed.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Facts to remove.</param>
        /// <returns>Result of facts removal.</returns>
        IFactResult TryRetractAll(IEnumerable<object> facts);

        /// <summary>
        /// Removes existing fact from the rules engine memory.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="fact">Fact to remove.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Retract(object fact);

        /// <summary>
        /// Removes a fact from the rules engine memory if the fact exists.
        /// </summary>
        /// <remarks>Bulk session operations are more performant than individual operations on a set of facts.</remarks>
        /// <param name="facts">Fact to remove.</param>
        /// <returns>Whether the fact was retracted or not.</returns>
        bool TryRetract(object facts);

        /// <summary>
        /// Starts rules execution cycle.
        /// This method blocks until there are no more rules to fire.
        /// </summary>
        /// <returns>Number of rules that fired.</returns>
        int Fire();

        /// <summary>
        /// Starts rules execution cycle.
        /// This method blocks until maximum number of rules fired or there are no more rules to fire.
        /// </summary>
        /// <param name="maxRulesNumber">Maximum number of rules to fire.</param>
        /// <returns>Number of rules that fired.</returns>
        int Fire(int maxRulesNumber);

        /// <summary>
        /// Creates a LINQ query to retrieve facts of a given type from the rules engine's memory.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to query. Use <see cref="object"/> to query all facts.</typeparam>
        /// <returns>Queryable working memory of the rules engine.</returns>
        IQueryable<TFact> Query<TFact>();
    }

    internal interface ISessionInternal : ISession
    {
        new IAgendaInternal Agenda { get; }

        IEnumerable<object> GetLinkedKeys(Activation activation);
        object GetLinked(Activation activation, object key);
        void InsertLinked(Activation activation, object key, object fact);
        void UpdateLinked(Activation activation, object key, object fact);
        void RetractLinked(Activation activation, object key, object fact);
    }

    /// <summary>
    /// See <see cref="ISession"/>.
    /// </summary>
    public sealed class Session : ISessionInternal, ISessionSnapshotProvider
    {
        private readonly IAgendaInternal _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IActionExecutor _actionExecutor;
        private readonly IExecutionContext _executionContext;

        internal Session(
            INetwork network,
            IAgendaInternal agenda,
            IWorkingMemory workingMemory,
            IEventAggregator eventAggregator,
            IActionExecutor actionExecutor,
            IIdGenerator idGenerator,
            IDependencyResolver dependencyResolver,
            IActionInterceptor actionInterceptor)
        {
            _network = network;
            _workingMemory = workingMemory;
            _agenda = agenda;
            _eventAggregator = eventAggregator;
            _actionExecutor = actionExecutor;
            _executionContext = new ExecutionContext(this, _workingMemory, _agenda, _eventAggregator, idGenerator);
            DependencyResolver = dependencyResolver;
            ActionInterceptor = actionInterceptor;
        }

        public IAgenda Agenda => _agenda;
        public IEventProvider Events => _eventAggregator;
        public IDependencyResolver DependencyResolver { get; set; }
        public IActionInterceptor ActionInterceptor { get; set; }

        IAgendaInternal ISessionInternal.Agenda => _agenda;

        internal void Activate()
        {
            _network.Activate(_executionContext);
        }

        public void InsertAll(IEnumerable<object> facts)
        {
            var result = TryInsertAll(facts);
            if (result.FailedCount > 0)
            {
                throw new ArgumentException("Facts for insert already exist", nameof(facts));
            }
        }

        public IFactResult TryInsertAll(IEnumerable<object> facts)
        {
            if (facts == null)
            {
                throw new ArgumentNullException(nameof(facts));
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var factObject in facts)
            {
                var factWrapper = _workingMemory.GetFact(factObject);
                if (factWrapper == null)
                {
                    factWrapper = new Fact(factObject);
                    toPropagate.Add(factWrapper);
                }
                else
                {
                    failed.Add(factObject);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                foreach (var fact in toPropagate)
                {
                    _workingMemory.AddFact(fact);
                }

                _network.PropagateAssert(_executionContext, toPropagate);
            }
            return result;
        }

        public void Insert(object fact)
        {
            InsertAll(new[] { fact });
        }

        public bool TryInsert(object fact)
        {
            var result = TryInsertAll(new[] {fact});
            return result.FailedCount == 0;
        }

        public void UpdateAll(IEnumerable<object> facts)
        {
            var result = TryUpdateAll(facts);
            if (result.FailedCount > 0)
            {
                throw new ArgumentException("Facts for update do not exist", nameof(facts));
            }
        }

        public IFactResult TryUpdateAll(IEnumerable<object> facts)
        {
            if (facts == null)
            {
                throw new ArgumentNullException(nameof(facts));
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var fact in facts)
            {
                var factWrapper = _workingMemory.GetFact(fact);
                if (factWrapper != null && factWrapper.Source == null)
                {
                    UpdateFact(factWrapper, fact);
                    toPropagate.Add(factWrapper);
                }
                else
                {
                    failed.Add(fact);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                foreach (var fact in toPropagate)
                {
                    _workingMemory.UpdateFact(fact);
                }

                _network.PropagateUpdate(_executionContext, toPropagate);
            }
            return result;
        }

        public void Update(object fact)
        {
            UpdateAll(new[] {fact});
        }

        public bool TryUpdate(object fact)
        {
            var result = TryUpdateAll(new[] {fact});
            return result.FailedCount == 0;
        }

        public void RetractAll(IEnumerable<object> facts)
        {
            var result = TryRetractAll(facts);
            if (result.FailedCount > 0)
            {
                throw new ArgumentException("Facts for retract do not exist", nameof(facts));
            }
        }

        public IFactResult TryRetractAll(IEnumerable<object> facts)
        {
            if (facts == null)
            {
                throw new ArgumentNullException(nameof(facts));
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var fact in facts)
            {
                var factWrapper = _workingMemory.GetFact(fact);
                if (factWrapper != null && factWrapper.Source == null)
                {
                    toPropagate.Add(factWrapper);
                }
                else
                {
                    failed.Add(fact);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                _network.PropagateRetract(_executionContext, toPropagate);

                foreach (var fact in toPropagate)
                {
                    _workingMemory.RemoveFact(fact);
                }
            }
            return result;
        }

        public void Retract(object fact)
        {
            RetractAll(new[] {fact});
        }

        public bool TryRetract(object fact)
        {
            var result = TryRetractAll(new[] {fact});
            return result.FailedCount == 0;
        }

        public IEnumerable<object> GetLinkedKeys(Activation activation)
        {
            var keys = _workingMemory.GetLinkedKeys(activation);
            return keys;
        }

        public object GetLinked(Activation activation, object key)
        {
            var factWrapper = _workingMemory.GetLinkedFact(activation, key);
            return factWrapper?.Object;
        }

        public void InsertLinked(Activation activation, object key, object fact)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (fact == null)
            {
                throw new ArgumentNullException(nameof(fact));
            }
            var factWrapper = _workingMemory.GetLinkedFact(activation, key);
            if (factWrapper != null)
            {
                throw new ArgumentException($"Linked fact already exists. Key={key}", nameof(fact));
            }
            factWrapper = new SyntheticFact(fact);
            factWrapper.Source = new LinkedFactSource(activation);
            _workingMemory.AddLinkedFact(activation, key, factWrapper);
            _network.PropagateAssert(_executionContext, new List<Fact> {factWrapper});
        }

        public void UpdateLinked(Activation activation, object key, object fact)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (fact == null)
            {
                throw new ArgumentNullException(nameof(fact));
            }
            var factWrapper = _workingMemory.GetLinkedFact(activation, key);
            if (factWrapper == null)
            {
                throw new ArgumentException($"Linked fact does not exist. Key={key}", nameof(fact));
            }
            factWrapper.Source = new LinkedFactSource(activation);
            _workingMemory.UpdateLinkedFact(activation, key, factWrapper, fact);
            _network.PropagateUpdate(_executionContext, new List<Fact> {factWrapper});
        }

        public void RetractLinked(Activation activation, object key, object fact)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (fact == null)
            {
                throw new ArgumentNullException(nameof(fact));
            }
            var factWrapper = _workingMemory.GetFact(fact);
            if (factWrapper == null)
            {
                throw new ArgumentException($"Linked fact does not exist. Key={key}", nameof(fact));
            }
            _network.PropagateRetract(_executionContext, new List<Fact> {factWrapper});
            _workingMemory.RemoveLinkedFact(activation, key, factWrapper);
            factWrapper.Source = null;
        }

        public int Fire()
        {
            return Fire(Int32.MaxValue);
        }

        public int Fire(int maxRulesNumber)
        {
            int ruleFiredCount = 0;
            while (!_agenda.IsEmpty() && ruleFiredCount < maxRulesNumber)
            {
                Activation activation = _agenda.Pop();
                IActionContext actionContext = new ActionContext(this, activation);

                _actionExecutor.Execute(_executionContext, actionContext);

                ruleFiredCount++;
                if (actionContext.IsHalted) break;
            }
            return ruleFiredCount;
        }

        public IQueryable<TFact> Query<TFact>()
        {
            return _workingMemory.Facts.Select(x => x.Object).OfType<TFact>().AsQueryable();
        }

        private static void UpdateFact(Fact fact, object factObject)
        {
            if (ReferenceEquals(fact.RawObject, factObject)) return;
            fact.RawObject = factObject;
        }

        SessionSnapshot ISessionSnapshotProvider.GetSnapshot()
        {
            var builder = new SnapshotBuilder();
            var visitor = new SessionSnapshotVisitor(_workingMemory);
            _network.Visit(builder, visitor);
            return builder.Build();
        }
    }
}
