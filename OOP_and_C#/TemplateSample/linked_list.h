#ifndef _LINKED_LIST_H_
#define _LINKED_LIST_H_

template <typename T> class Node
{
	Node* next;
	Node* prev;

public:
	Node();
	virtual ~Node();

	void SetNext(Node*);
	Node* GetNext() const;
	void SetPrev(Node*);
	Node* GetPrev() const;
};

template <typename T> class LinkedList
{
	Node<T>* head;
	Node<T>* tail;

public:
	LinkedList() :
		head(nullptr),
		tail(nullptr)
	{}

	void AddNode(Node<T>* node);
	Node<T>* GetLast();
};


#endif