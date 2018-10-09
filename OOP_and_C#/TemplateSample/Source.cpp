#include "train.h"


int main()
{
	LinkedList<Train>* train = new LinkedList<Train>();
	train->AddNode(new Train());
	Train* last = dynamic_cast<Train*>(train->GetLast());
}