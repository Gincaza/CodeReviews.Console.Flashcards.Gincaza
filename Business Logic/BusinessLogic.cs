using Business_Logic.Interfaces;

namespace Business_Logic;

class BusinessLogic
{
    private IDataAcess? dataAccess;

    public BusinessLogic(IDataAcess? dataAccess)
    {
        this.dataAccess = dataAccess;
    }
}