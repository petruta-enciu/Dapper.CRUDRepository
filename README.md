Dapper.CRUDRepository
============

Similar POCO and DbContext design approach, this project intends to implement an Entity CRUD Repository using Dapper and Dapper Extensions.
This project will add an additional layer above existing data context layer represented by IDatabase, IDapperImplementor and ISqlDialect DapperExtentions.

Repository will take advantage of abstractization level offered by:
- DapperExtentions.Database class implementation and IDatabase public interface suitable for dependency injection;
- DapperExtentions.DapperImplementor class and IDapperImplementor public interface suitable for dependency injection;
- DapperExtentions .SqlServerDialect, .MySqlDialect, .SqlCeDialect, .SqliteDialect and ISqlDialect public interface suitable for dependency injection;

Dapper-dot-net packages could be found here https://github.com/SamSaffron/dapper-dot-net.
Dapper-Extensions packages could be found here https://github.com/tmsmith/Dapper-Extensions.


