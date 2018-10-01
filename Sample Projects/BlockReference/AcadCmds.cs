using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;

[assembly: CommandClass(typeof(UseBlkRef.AcadCmds))]
namespace UseBlkRef
{
	public class AcadCmds
	{
		[CommandMethod("CreateBlock")]
		public void CreateBlock()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				// Get the block table from the drawing
				BlockTable blk_tbl = (BlockTable)tr.GetObject(
					AcadFuncs.GetActiveDb().BlockTableId,
					OpenMode.ForRead);

				PromptStringOptions pso = new PromptStringOptions("\nEnter new block name: ");
				pso.AllowSpaces = true;
				string blk_name = "";
				do
				{
					PromptResult pr = AcadFuncs.GetEditor().GetString(pso);
					if (PromptStatus.OK != pr.Status)
						return;

					try
					{
						SymbolUtilityServices.ValidateSymbolName(pr.StringResult, false);
						if (blk_tbl.Has(pr.StringResult))
							AcadFuncs.GetEditor().WriteMessage("\nA block with this name already exists.");
						else
							blk_name = pr.StringResult;
					}
					catch
					{
						AcadFuncs.GetEditor().WriteMessage("\nInvalid block name.");
					}
				} while ("" == blk_name);

				BlockTableRecord btr = new BlockTableRecord();
				btr.Name = blk_name;

				blk_tbl.UpgradeOpen();
				ObjectId btrId = blk_tbl.Add(btr);
				tr.AddNewlyCreatedDBObject(btr, true);

				DBObjectCollection ents = SquareOfLines(5);
				foreach (Entity ent in ents)
				{
					btr.AppendEntity(ent);
					tr.AddNewlyCreatedDBObject(ent, true);
				}

				BlockTableRecord ms = (BlockTableRecord)tr.GetObject(blk_tbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
				BlockReference br = new BlockReference(AcadGeo.Point3d.Origin, btrId);
				ms.AppendEntity(br);
				tr.AddNewlyCreatedDBObject(br, true);
				tr.Commit();
			}
		}

		private DBObjectCollection SquareOfLines(double size)
		{
			DBObjectCollection ents = new DBObjectCollection();
			AcadGeo.Point3d[] pts = {
				new AcadGeo.Point3d(-size, -size, 0),
				new AcadGeo.Point3d(size, -size, 0),
				new AcadGeo.Point3d(size, size, 0),
				new AcadGeo.Point3d(-size, size, 0)};

			int max = pts.GetUpperBound(0);
			for (int i = 0; i <= max; i++)
			{
				int j = (i == max ? 0 : i + 1);
				Line ln = new Line(pts[i], pts[j]);
				ents.Add(ln);
			}
			return ents;
		}

		[CommandMethod("AddingAttributeToABlock")]
		public void AddingAttributeToABlock()
		{
			// Get the current database and start a transaction
			using (Transaction acTrans = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				// Open the Block table for read
				BlockTable acBlkTbl;
				acBlkTbl = acTrans.GetObject(AcadFuncs.GetActiveDb().BlockTableId, OpenMode.ForRead) as BlockTable;

				if (!acBlkTbl.Has("CircleBlockWithAttributes"))
				{
					using (BlockTableRecord blk_tbl_rcd = new BlockTableRecord())
					{
						blk_tbl_rcd.Name = "CircleBlockWithAttributes";
						blk_tbl_rcd.Origin = new AcadGeo.Point3d(0, 0, 0);

						using (Circle circle = new Circle())
						{
							circle.Center = new AcadGeo.Point3d(0, 0, 0);
							circle.Radius = 2;

							blk_tbl_rcd.AppendEntity(circle);

							// Add an attribute definition to the block
							using (AttributeDefinition att_def = new AttributeDefinition())
							{
								att_def.Position = new AcadGeo.Point3d(0, 0, 0);
								att_def.Verifiable = true;
								att_def.Prompt = "Door #: ";
								att_def.Tag = "Door#";
								att_def.TextString = "DXX";
								att_def.Height = 1;
								att_def.Justify = AttachmentPoint.MiddleCenter;

								blk_tbl_rcd.AppendEntity(att_def);

								acBlkTbl.UpgradeOpen();
								acBlkTbl.Add(blk_tbl_rcd);
								acTrans.AddNewlyCreatedDBObject(blk_tbl_rcd, true);
							}
						}
					}
				}

				// Save the new object to the database
				acTrans.Commit();

				// Dispose of the transaction
			}
		}

		[CommandMethod("GettingAttributes")]
		public void GettingAttributes()
		{
			using (Transaction tran = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				// Open the Block table for read
				BlockTable acBlkTbl;
				acBlkTbl = tran.GetObject(AcadFuncs.GetActiveDb().BlockTableId, OpenMode.ForRead) as BlockTable;

				ObjectId blkRecId = ObjectId.Null;

				if (!acBlkTbl.Has("TESTBLOCK"))
				{
					using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
					{
						acBlkTblRec.Name = "TESTBLOCK";

						// Set the insertion point for the block
						acBlkTblRec.Origin = new AcadGeo.Point3d(0, 0, 0);

						// Add an attribute definition to the block
						using (AttributeDefinition acAttDef = new AttributeDefinition())
						{
							acAttDef.Position = new AcadGeo.Point3d(5, 5, 0);
							acAttDef.Prompt = "1234";
							acAttDef.Tag = "My Att";
							acAttDef.TextString = "4321";
							acAttDef.Height = 1;
							acAttDef.Justify = AttachmentPoint.MiddleCenter;
							acBlkTblRec.AppendEntity(acAttDef);

							acBlkTbl.UpgradeOpen();
							acBlkTbl.Add(acBlkTblRec);
							tran.AddNewlyCreatedDBObject(acBlkTblRec, true);
						}

						blkRecId = acBlkTblRec.Id;
					}
				}
				else
				{
					blkRecId = acBlkTbl["CircleBlockWithAttributes"];
				}

				// Create and insert the new block reference
				if (blkRecId != ObjectId.Null)
				{
					BlockTableRecord acBlkTblRec;
					acBlkTblRec = tran.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;

					using (BlockReference acBlkRef = new BlockReference(new AcadGeo.Point3d(5, 5, 0), acBlkTblRec.Id))
					{
						BlockTableRecord acCurSpaceBlkTblRec;
						acCurSpaceBlkTblRec = tran.GetObject(AcadFuncs.GetActiveDb().CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

						acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
						tran.AddNewlyCreatedDBObject(acBlkRef, true);

						// Verify block table record has attribute definitions associated with it
						if (acBlkTblRec.HasAttributeDefinitions)
						{
							// Add attributes from the block table record
							foreach (ObjectId objID in acBlkTblRec)
							{
								DBObject dbObj = tran.GetObject(objID, OpenMode.ForRead) as DBObject;

								if (dbObj is AttributeDefinition)
								{
									AttributeDefinition acAtt = dbObj as AttributeDefinition;

									if (!acAtt.Constant)
									{
										using (AttributeReference acAttRef = new AttributeReference())
										{
											acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
											acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);

											acAttRef.TextString = acAtt.TextString;

											acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
											tran.AddNewlyCreatedDBObject(acAttRef, true);
										}
									}
								}
							}

							// Display the tags and values of the attached attributes
							string message = "";
							AttributeCollection atts = acBlkRef.AttributeCollection;

							foreach (ObjectId obj_id in atts)
							{
								AttributeReference att_ref = tran.GetObject(obj_id, OpenMode.ForRead) as AttributeReference;

								message = message + "Tag: " + att_ref.Tag + "\n" +
												"Value: " + att_ref.TextString + "\n";
							}

							AcadApp.Application.ShowAlertDialog("The attributes for blockReference " + acBlkRef.Name + " are:\n" + message);
						}
					}
				}

				// Save the new object to the database
				tran.Commit();

				// Dispose of the transaction
			}
		}
	}
}
