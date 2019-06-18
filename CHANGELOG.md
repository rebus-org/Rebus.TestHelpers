# Changelog

## 5.0.0

* Initial version
* Add fake implementations of everything, including lots of new event types
* Collect handler exceptions in saga fixture

## 5.1.0

* Add `DeliverFailed` method to `SagaFixture<>`, making is possible to simulate 2nd level dispatch (i.e. dispatching as `IFailed<TMessage>`)

## 5.2.0

* Add `PrepareConflict<>` method to `SagaFixture<>`, enabling controlled simulation of saga data update conflicts

## 5.2.1

* Fix type of event recorded when a message is deferred and explicitly routed at the same time - thanks [rsivanov]

## 5.2.2

* Add `FakeMessageContext` to make it easier to simulate message headers in tests

[rsivanov]: https://github.com/rsivanov