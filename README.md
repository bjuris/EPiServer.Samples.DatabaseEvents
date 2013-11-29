EPiServer.Samples.DatabaseEvents
================================

Sample implementation of the event system in CMS that uses a database table to route events.

Download code and compile, run Database.sql to get the table in place. Then add configuration to EPiServerFramework.config:

```xml
  <event defaultProvider="db">
    <providers>
      <clear/>
      <add name="db" type="EPiServer.Samples.DatabaseEvents.DbEventProvider, EPiServer.Samples.DatabaseEvents"/>
    </providers>
  </event>
```
