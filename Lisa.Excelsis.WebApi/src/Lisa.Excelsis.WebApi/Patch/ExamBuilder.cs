using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class ExamBuilder : Patcher
    {
        public void BuildPatches(int id, IEnumerable<Patch> patches)
        {
            transactions = new List<QueryData>();

            foreach (Patch patch in patches)
            {
                //Add Category
                Build("add", id, patch, @"^categories$", AddCategory);
                //Add Criterion
                Build("add", id, patch, @"^categories/\d/criteria$", AddCriterion);

                //Replace Category
                Build("replace", id, patch, @"^categories/\d/order$", ReplaceCategoryOrder);
                Build("replace", id, patch, @"^categories/\d/name$", ReplaceCategoryName);
                //Replace Criterion
                Build("replace", id, patch, @"^categories/\d/criteria/\d/order$", ReplaceCriterionOrder);
                Build("replace", id, patch, @"^categories/\d/criteria/\d/title$", ReplaceCriterionTitle);
                Build("replace", id, patch, @"^categories/\d/criteria/\d/description$", ReplaceCriterionDescription);
                Build("replace", id, patch, @"^categories/\d/criteria/\d/weight$", ReplaceCriterionWeight);
                //Replace Exam
                Build("replace", id, patch, @"^subject$", ReplaceExamSubject);
                Build("replace", id, patch, @"^name$", ReplaceExamName);
                Build("replace", id, patch, @"^cohort$", ReplaceExamCohort);
                Build("replace", id, patch, @"^crebo$", ReplaceExamCrebo);
                Build("replace", id, patch, @"^status$", ReplaceExamStatus);

                //Remove Category
                Build("remove", id, patch, @"^categories$", RemoveCategory);
                Build("remove", id, patch, @"^categories/\d/criteria$", RemoveCriterion);

                //Move Criterion
                Build("move", id, patch, @"^categories/\d/criteria/\d$", MoveCriterion);
            }

            ExecuteTransactions();
        }

        //Add
        private static QueryData AddCategory(int id, Patch patch)
        {
            QueryData data = new QueryData();
            CategoryAdd category = patch.Value.ToObject<CategoryAdd>();
            if (category != null)
            {
                category.ExamId = id;
                data.Query = @" INSERT INTO Categories ([Order],[Name],[ExamId])
                                VALUES (@Order, @Name, @ExamId)";
                data.Parameters = category;
                return data;
            }
            return new QueryData();
        }
        
        private static QueryData AddCriterion(int id, Patch patch)
        {
            QueryData data = new QueryData();
            CriterionAdd criterion = patch.Value.ToObject<CriterionAdd>();
            if (criterion != null)
            {
                criterion.ExamId = id;
                criterion.CategoryId = Convert.ToInt32(patch.Field.Split('/')[1]);
                data.Query = @" INSERT INTO Criteria ([Order],[Title],[Description],[Weight],[ExamId],[CategoryId])
                                VALUES (@Order, @Title, @Description, @Weight, @ExamId, @CategoryId)";
                data.Parameters = criterion;
                return data;
            }
            return new QueryData();
        }

        //Replace
        private static QueryData ReplaceCategoryOrder(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Categories] SET [Order] = @Order WHERE [Id] = @Cid";
            data.Parameters = new { Order = patch.Value.ToString(), Cid = field[1] };
            return data;
        }

        private static QueryData ReplaceCategoryName(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Categories] SET [Name] = @Name WHERE [Id] = @Cid";
            data.Parameters = new { Name = patch.Value.ToString(), Cid = field[1] };
            return data;
        }

        private static QueryData ReplaceCriterionOrder(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Criteria] SET [Order] = @Order WHERE [Id] = @Cid";
            data.Parameters = new { Name = patch.Value.ToString(), Cid = field[3] };
            return data;
        }

        private static QueryData ReplaceCriterionTitle(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Criteria] SET [Title] = @Title WHERE [Id] = @Cid";
            data.Parameters = new { Title = patch.Value.ToString(), Cid = field[3] };
            return data;
        }

        private static QueryData ReplaceCriterionDescription(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Criteria] SET [Description] = @Description WHERE [Id] = @Cid";
            data.Parameters = new { Description = patch.Value.ToString(), Cid = field[3] };
            return data;
        }

        private static QueryData ReplaceCriterionWeight(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Field.Split('/');
            data.Query = @"UPDATE [Criteria] SET [Weight] = @Weight WHERE [Id] = @Cid";
            data.Parameters = new { Weight = patch.Value.ToString(), Cid = field[3] };
            return data;
        }

        private static QueryData ReplaceExamSubject(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Exams] SET [Subject] = @Subject WHERE [Id] = @Eid;
                           UPDATE [Exams] SET [SubjectId] = @SubjectId WHERE [Id] = @Eid;";
            data.Parameters = new { Subject = patch.Value.ToString(), SubjectId = Misc.CleanParam(patch.Value.ToString()), Eid = id };
            return data;
        }

        private static QueryData ReplaceExamName(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Exams] SET [Name] = @Name WHERE [Id] = @Eid;
                           UPDATE [Exams] SET [NameId] = @NameId WHERE [Id] = @Eid;";
            data.Parameters = new { Name = patch.Value.ToString(), NameId = Misc.CleanParam(patch.Value.ToString()), Eid = id };
            return data;
        }

        private static QueryData ReplaceExamCohort(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Exams] SET [Cohort] = @Cohort WHERE [Id] = @Eid";
            data.Parameters = new { Cohort = patch.Value.ToString(), Eid = id };
            return data;
        }

        private static QueryData ReplaceExamCrebo(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Exams] SET [Crebo] = @Crebo WHERE [Id] = @Eid";
            data.Parameters = new { Crebo = patch.Value.ToString(), Eid = id };
            return data;
        }

        private static QueryData ReplaceExamStatus(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"UPDATE [Exams] SET [Status] = @Status WHERE [Id] = @Eid";
            data.Parameters = new { Status = patch.Value.ToString(), Eid = id };
            return data;
        }

        // Remove
        private static QueryData RemoveCategory(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"DELETE FROM [Categories] WHERE [Id] = @Cid";
            data.Parameters = new { Cid = patch.Value.ToString() };
            return data;
        }

        private static QueryData RemoveCriterion(int id, Patch patch)
        {
            QueryData data = new QueryData();
            data.Query = @"DELETE FROM [Criteria] WHERE [Id] = @Cid";
            data.Parameters = new { Cid = patch.Value.ToString() };
            return data;
        }

        // Move
        private static QueryData MoveCriterion(int id, Patch patch)
        {
            QueryData data = new QueryData();
            var field = patch.Target.Split('/');
            data.Query = @"UPDATE [Criteria] SET [CategoryId] = @CategoryId WHERE [Id] = @Cid";
            data.Parameters = new { CategoryId = field[1], Cid = patch.Value.ToString() };
            return data;
        }
    }
}
