#ifndef _TRAIN_H_
#define _TRAIN_H_

#include "linked_list.h"

class Train : public Node<Train>
{
	int a;
	int b;
	double c;
	float d;
public:
	Train() : Node<Train>()
	{}
};

#endif